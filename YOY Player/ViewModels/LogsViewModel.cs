using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YOYPlayer.Model;
using YOYPlayer.Model.Utils;

namespace YOYPlayer.ViewModels
{
    public class LogsViewModel : IViewModel
    {
        public bool IsLocked { get; set; }

        public LogsViewModel()
        {
            BackCommand = new RelayCommand(Back);
        }

        #region ICommands

        public ICommand BackCommand { get; set; }

        #endregion

        #region ICommands implementations

        private void Back()
        {
            NavigationService.Instance.GoBack();
        }

        #endregion
    }
}
