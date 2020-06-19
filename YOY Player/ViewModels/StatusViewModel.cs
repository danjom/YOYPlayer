using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YOYPlayer.Model;
using YOYPlayer.Model.Utils;
using YOYPlayer.Model.Utils.Enums;

namespace YOYPlayer.ViewModels
{
    public class StatusViewModel : IViewModel
    {
        public bool IsLocked { get; set; }

        public StatusViewModel()
        {
            OpenLogsCommand = new RelayCommand(OpenLogs);
            OpenAuthCommand = new RelayCommand(OpenAuth);
            OpenFilesCommand = new RelayCommand(OpenFiles);
        }

        #region ICommands

        public ICommand OpenLogsCommand { get; set; }
        public ICommand OpenAuthCommand { get; set; }
        public ICommand OpenFilesCommand { get; set; }

        #endregion

        #region Commands implementations

        private void OpenLogs()
        {
            NavigationService.Instance.Navigate(new LogsViewModel());
        }

        private void OpenAuth()
        {
            NavigationService.Instance.Navigate(new AuthViewModel(AfterLoginAction.Exit));
        }

        private void OpenFiles()
        {
            NavigationService.Instance.Navigate(new FilesViewModel(AfterFileChangeAction.Back));
        }

        #endregion
    }
}
