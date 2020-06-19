using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YOYPlayer.Model;
using YOYPlayer.Model.Helpers;
using YOYPlayer.Model.Utils;
using YOYPlayer.Model.Utils.Enums;

namespace YOYPlayer.ViewModels
{
    public class ToolTipViewModel : IViewModel
    {
        public bool IsLocked { get; set; }

        public ToolTipViewModel()
        {
            ShowStatusCommand = new RelayCommand(OpenStatusWin);
        }

        #region ICommands

        public ICommand ShowStatusCommand { get; set; }

        #endregion

        #region ICommands implementations

        private void OpenStatusWin()
        {
            if (UIHelper.IsWindowOpened)
                UIHelper.CloseWindow();
            else
            {
                if (string.IsNullOrEmpty(Properties.Settings.Default.OAuthToken) || DateTime.Now > YOYPlayer.Properties.Settings.Default.OAuthExpired)
                    UIHelper.OpenVM(new AuthViewModel(AfterLoginAction.Start));
                else
                    UIHelper.OpenVM(new StatusViewModel());
            }
        }

        #endregion
    }
}
