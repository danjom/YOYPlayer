using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using YOYPlayer.Model;
using YOYPlayer.Model.Helpers;
using YOYPlayer.Model.Utils;
using YOYPlayer.Model.Utils.Enums;
using YOYPlayer.Model.Utils.Exceptions;

namespace YOYPlayer.ViewModels
{
    public class AuthViewModel : VerifiedViewModel
    {
        #region Fields

        private AfterLoginAction _action;

        #endregion

        #region Properties

        public string Username
        {
            get { return Get(() => Username); }
            set { Set(() => Username, value); }
        }
        public string Password
        {
            get { return Get(() => Password); }
            set { Set(() => Password, value); }
        }

        public bool IsInWait { get; set; } = false;

        #endregion

        public AuthViewModel(AfterLoginAction action)
        {
            _action = action;

            BackCommand = new RelayCommand(Back);
            LoginCommand = new RelayCommand(Login, _ => !HasErrors);
            OpenLinkCommand = new RelayCommand<Uri>(OpenLink);

            InitValidationRules();

            Password = string.Empty;
            Username = string.Empty;
        }

        private void InitValidationRules()
        {
            AddRule(
                () => Username,
                () => Username.Length > 0 && StringHelper.IsEmail(Username),
                "Void username.");
            AddRule(
                () => Password,
                () => Password.Length > 0,
                "Void password.");
        }

        #region ICommands

        public ICommand BackCommand { get; set; }
        public ICommand LoginCommand { get; set; }

        public ICommand OpenLinkCommand { get; set; }

        #endregion

        #region ICommands implementations

        private void OpenLink(Uri obj)
        {
            Process.Start(new ProcessStartInfo(obj.ToString()));
        }

        private void Back()
        {
            NavigationService.Instance.GoBack();
        }

        private async void Login()
        {
            try
            {
                IsInWait = true;
                Notify("IsInWait");

                var result = await WebHelper.GetNewOAuthToken(Username, Password);

                switch (_action)
                {
                    //case AfterLoginAction.Exit:

                    //    Application.Current.Shutdown();

                    //    break;
                    case AfterLoginAction.Start:
                    case AfterLoginAction.Exit:

                        Properties.Settings.Default.OAuthToken = result.AccessToken;
                        Properties.Settings.Default.OAuthExpired = TypeHelper.DateFromExpired(result.ExpiresIn); // DateTime.Parse(result.ExpiresIn);
                        Properties.Settings.Default.Username = Username;
                        Properties.Settings.Default.Save();

                        NavigationService.Instance.Navigate(new FilesViewModel(AfterFileChangeAction.Open));
                        
                        break;
                }
            }
            catch (WrongAuthDataException)
            {
                MessageBox.Show(ResourcesHelper.GetString("Auth_Message_WrongCredentials"));
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
