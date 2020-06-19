using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using YOYPlayer.Controls;
using YOYPlayer.ViewModels;

namespace YOYPlayer.Model.Helpers
{
    public static class UIHelper
    {
        private static DialogWindow _currentWin = null;
        private static object locker = new object();

        public static bool IsWindowOpened { get => _currentWin != null; }
        public static Thread MainUIThread { get; set; }

        public static void OpenVM<TVM>(TVM viewModel) where TVM : IViewModel
        {
            lock (locker)
                if (_currentWin == null)
                {
                    Dispatcher.FromThread(MainUIThread).Invoke(() =>
                    {
                        var dw = new DialogWindow()
                        {
                            Content = viewModel
                        };
                        _currentWin = dw;
                        dw.Show();
                        dw.Closed += (_, __) => { _currentWin = null; };
                        dw.Activate();
                    });
                }
                else
                {
                    _currentWin.Activate();
                }
        }

        public static void OpenVMAsDialog<TVM>(TVM viewModel) where TVM : IViewModel
        {
            lock (locker)
                if (_currentWin == null)
                {
                    var dw = new DialogWindow()
                    {
                        Content = viewModel
                    };
                    dw.Closed += (_, __) =>
                    {
                        _currentWin = null;
                    };
                    _currentWin = dw;
                    dw.ShowDialog();
                }
                else
                {
                    _currentWin.Activate();
                }
        }

        //private static Thread _uiThread = null;
        //public static void OpenVMLocked<TVM>(TVM viewModel) where TVM : IViewModel
        //{
        //    viewModel.IsLocked = true;

        //    lock (locker)
        //        if (_currentWin == null)
        //        {
        //            _uiThread = new Thread(new ThreadStart(() =>
        //            {
        //                var dw = new DialogWindow()
        //                {
        //                    Content = viewModel
        //                };
        //                _currentWin = dw;
        //                dw.Show();
        //                dw.Closed += (_, __) => 
        //                {
        //                    viewModel.IsLocked = false;
        //                    _currentWin = null;
        //                };
        //            }));
        //            _uiThread.SetApartmentState(ApartmentState.STA);
        //            _uiThread.Start();

        //            while (viewModel.IsLocked)
        //            {
        //                Thread.Sleep(200);
        //            }
        //        }
        //        else
        //        {
        //            _currentWin.Activate();
        //        }
        //}

        public static void ActivateWindow()
        {
            lock (locker)
                if (_currentWin != null)
                    _currentWin.Activate();
        }

        public static void CloseWindow()
        {
            lock (locker)
                if (_currentWin != null)
                {
                    _currentWin.Close();
                    _currentWin = null;
                }
        }
    }
}
