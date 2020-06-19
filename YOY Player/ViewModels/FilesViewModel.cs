using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using YOYPlayer.Model;
using YOYPlayer.Model.Data;
using YOYPlayer.Model.Helpers;
using YOYPlayer.Model.Utils;
using YOYPlayer.Model.Utils.Enums;
using YOYPlayer.Model.Utils.Exceptions;

namespace YOYPlayer.ViewModels
{
    public class FilesViewModel : VerifiedViewModel
    {
        #region Properties

        private Commerce _selectedCommerce;
        private AfterFileChangeAction _action;

        public List<Commerce> Commerces { get; set; }
        public Commerce SelectedCommerce
        {
            get => _selectedCommerce;
            set
            {
                _selectedCommerce = value;
                Notify("SelectedCommerce");
            }
        }
        public Branche SelectedBranche { get; set; }
        public Department SelectedDepartment { get; set; }

        public bool IsInWait { get; set; } = false;

        #endregion

        public FilesViewModel(AfterFileChangeAction action)
        {
            _action = action;

            BackCommand = new RelayCommand(Back);
            RunCommand = new RelayCommand(Run, _ => !HasErrors);

            InitValidationRules();

            Task.Factory.StartNew(() => InitData());
        }

        private void InitValidationRules()
        {
            AddRule(
                () => SelectedCommerce,
                () => SelectedCommerce != null,
                "");

            AddRule(
                () => SelectedBranche,
                () => SelectedBranche != null,
                "");

            AddRule(
                () => SelectedDepartment,
                () => SelectedDepartment != null,
                "");
        }

        private async void InitData()
        {
            try
            {
                IsInWait = true;
                Notify("IsInWait");

                Commerces = await WebHelper.GetCommerces();
            }
            catch (AuthTokenExpiredException)
            {
                Player.Stop();
                MessageBox.Show(ResourcesHelper.GetString("Auth_Message_Expired"));

                NavigationService.Instance.NavigateForsed(new AuthViewModel(AfterLoginAction.Start));
            }
            catch (Exception)
            {
                MessageBox.Show(ResourcesHelper.GetString("Response_Error"));
                //NavigationService.Instance.NavigateForsed(new AuthViewModel(AfterLoginAction.Start));
            }
            finally
            {
                IsInWait = false;
                Notify("IsInWait");
                Notify("Commerces");
            }
        }

        #region ICommands

        public ICommand BackCommand { get; set; }
        public ICommand RunCommand { get; set; }

        #endregion

        #region ICommands implementations

        private void Back()
        {
            NavigationService.Instance.GoBack();
        }

        private async void Run()
        {
            try
            {
                IsInWait = true;
                Notify("IsInWait");

                var result = await WebHelper.GetAudioInfo(SelectedCommerce, SelectedBranche, SelectedDepartment);

                if (Player.Instance.IsActive)
                {
                    Player.Stop();

                    var path = Path.Combine(FoldersHelper.PathToAudio, Properties.Settings.Default.SelectedFile);

                    if (File.Exists(path))
                        File.Delete(path);
                }
                
                await WebHelper.LoadAudio(result, SelectedBranche.AccessKey);

                Properties.Settings.Default.SelectedFile = result.FileName;
                Properties.Settings.Default.CurentFileExpired = DateTime.Parse(result.NextSync);

                Properties.Settings.Default.AudioInfo = new AudioInfo()
                {
                    AccessKey = SelectedBranche.AccessKey,

                    BranchId = SelectedBranche.ID,
                    BranchName = SelectedBranche.Name,

                    CommerceId = SelectedCommerce.ID,
                    CommerceName = SelectedCommerce.Name,

                    DepartmentId = SelectedDepartment.ID,
                    DepartmentName = SelectedDepartment.Name,

                    FileID = result.FileId,
                };

                Properties.Settings.Default.Save();

                Player.RunLoop(Path.Combine(FoldersHelper.PathToAudio, result.FileName));

                switch (_action)
                {
                    case AfterFileChangeAction.Back:

                        NavigationService.Instance.GoBack();

                        break;
                    case AfterFileChangeAction.Open:

                        NavigationService.Instance.Navigate(new StatusViewModel());

                        break;
                }
            }
            catch (AuthTokenExpiredException)
            {
                Player.Stop();
                MessageBox.Show(ResourcesHelper.GetString("Auth_Message_Expired"));

                NavigationService.Instance.NavigateForsed(new AuthViewModel(AfterLoginAction.Start));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsInWait = false;
                Notify("IsInWait");
            }
        }

        #endregion
    }
}
