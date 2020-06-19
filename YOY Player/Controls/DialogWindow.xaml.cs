using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YOYPlayer.Model;
using YOYPlayer.Model.Helpers;

namespace YOYPlayer.Controls
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        Boolean bLoadedFirstTime = false;
        public DialogWindow()
        {
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.System && e.SystemKey == Key.F4)
                e.Handled = true;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            NavigationService.Instance.UnRegisterFrame();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NavigationService.Instance.RegisterFrame(this);            
        }
       
        private void Window_StateChanged(object sender, EventArgs e)
        {
            UIHelper.CloseWindow();
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
            }
            else if (this.WindowState == WindowState.Normal)
            {                
                this.ShowInTaskbar = true;
            }
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (GlobalData.bLoadedFirstTime )
            {
                System.Threading.Thread.Sleep(3000);
                this.WindowState = WindowState.Minimized;
                GlobalData.bLoadedFirstTime = false;
            }
        }

    }
}
