using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;
using YOYPlayer.Controls;
using YOYPlayer.Model.Helpers;
using YOYPlayer.ViewModels;

namespace YOYPlayer.Model
{
    public class NavigationService : INavigationService
    {
        #region Fields

        private DialogWindow _currentWin = null;
        private List<IViewModel> _history = null;

        #endregion

        #region Properties

        public bool IsHead => (_history?.Count ?? 1) == 1;

        #endregion

        #region Singletone

        private static NavigationService _instance = null;
        public static NavigationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NavigationService();
                }

                return _instance;
            }
        }

        #endregion

        #region Public Methods

        public void RegisterFrame(DialogWindow win)
        {
            _currentWin = win;
            _history = new List<IViewModel>();
            _history.Add(win.Content as IViewModel);
        }
        public void UnRegisterFrame()
        {
            _currentWin = null;
            _history = null;
        }

        #endregion

        public void GoBack()
        {
            Dispatcher.FromThread(UIHelper.MainUIThread).Invoke(() =>
            {
                _history.Remove(_history.Last());
                _currentWin.Content = _history.Last();
            });
        }

        public void Navigate(IViewModel model)
        {
            Dispatcher.FromThread(UIHelper.MainUIThread).Invoke(() =>
            {
                _currentWin.Content = model;
                _history.Add(model);
            });
        }

        public void NavigateForsed(IViewModel model)
        {
            _history.Clear();

            Dispatcher.FromThread(UIHelper.MainUIThread).Invoke(() =>
            {
                _currentWin.Content = model;
                _history.Add(model);
            });
        }
    }

    public interface INavigationService
    {
        void GoBack();
        void Navigate(IViewModel model);
    }
}
