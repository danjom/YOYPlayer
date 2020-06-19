using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Shell;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using YOYPlayer.Model;
using YOYPlayer.Model.Data;
using YOYPlayer.Model.Helpers;
using YOYPlayer.Model.Utils.Enums;
using YOYPlayer.Resources.Strings;
using YOYPlayer.ViewModels;

namespace YOYPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, ISingleInstanceApp
    {
        [STAThread]
        public static void Main()
        {
            if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
            {
                var application = new App();
                application.InitializeComponent();
                application.Run();

                // Allow single instance code to perform cleanup operations
                SingleInstance<App>.Cleanup();
            }
        }

        private const string Unique = "4EABFF23-A35E-F0AB-3189-C81203BCAFF1";
        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Length == 1 && e.Args[0] == "INSTALLER")
            {
                Process.Start(Assembly.GetExecutingAssembly().Location);
                Application.Current.Shutdown();

                return;
            }

            //This means, app just started
            GlobalData.bLoadedFirstTime = true;

            Start();

            base.OnStartup(e);
        }

        private void Start()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folder2 = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            Strings.Culture = new CultureInfo("es-ES");

            UIHelper.MainUIThread = Thread.CurrentThread;

            var ni = (TaskbarIcon)FindResource("appNotifyIcon");

            try
            {
                var token = YOYPlayer.Properties.Settings.Default.OAuthToken;

                if (DateTime.Now > YOYPlayer.Properties.Settings.Default.OAuthExpired)
                {
                    UIHelper.OpenVMAsDialog(new AuthViewModel(AfterLoginAction.Start));
                }
                else
                {
                    if (!File.Exists(Path.Combine(FoldersHelper.PathToAudio, YOYPlayer.Properties.Settings.Default.SelectedFile)) ||
                        DateTime.Now > YOYPlayer.Properties.Settings.Default.CurentFileExpired)
                    {
                        UIHelper.OpenVMAsDialog(new FilesViewModel(AfterFileChangeAction.Open));
                    }
                    else
                    {
                        Player.RunLoop(Path.Combine(FoldersHelper.PathToAudio, YOYPlayer.Properties.Settings.Default.SelectedFile));

                        UIHelper.OpenVMAsDialog(new StatusViewModel());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Player.Stop();

            base.OnExit(e);
        }

        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Exception ex = default(Exception);
            ex = e.Exception;

            StringBuilder errorDetails = new StringBuilder();

            while (ex != null)
            {
                errorDetails.Append($"{ex.Message}{Environment.NewLine}");
                ex = ex.InnerException;
            }

            errorDetails.Append($"==={Environment.NewLine}{e.Exception.StackTrace}");

            File.WriteAllText("FatalError.txt", errorDetails.ToString());
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = default(Exception);
            ex = (Exception)e.ExceptionObject;

            StringBuilder errorDetails = new StringBuilder();

            while (ex != null)
            {
                errorDetails.Append($"{ex.Message}{Environment.NewLine}");
                ex = ex.InnerException;
            }

            errorDetails.Append($"==={Environment.NewLine}{((Exception)e.ExceptionObject).StackTrace}");

            File.WriteAllText("FatalError.txt", errorDetails.ToString());
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            if (!UIHelper.IsWindowOpened)
            {
                if (string.IsNullOrEmpty(YOYPlayer.Properties.Settings.Default.OAuthToken) || DateTime.Now > YOYPlayer.Properties.Settings.Default.OAuthExpired)
                    UIHelper.OpenVM(new AuthViewModel(AfterLoginAction.Start));
                else
                    UIHelper.OpenVM(new StatusViewModel());
            }
            else
                UIHelper.ActivateWindow();

            return true;
        }
    }
}