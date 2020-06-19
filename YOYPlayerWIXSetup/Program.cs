using Microsoft.Deployment.WindowsInstaller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WixSharp;
using WixSharp.Controls;

namespace YOYPlayerWIXSetup
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YOYPlayer");
            

            Project project = new Project("YOY Player",

                new Dir(@"%ProgramFiles%\YOY Player",
                    new File(@"..\YOY Player\bin\Release\YOYPlayer.exe",
                        new FileShortcut("YOYPlayer", @"%Desktop%"),
                        new FileShortcut("YOYPlayer", @"%Startup%")),
                    new File(@"..\YOY Player\bin\Release\WpfAnimatedGif.dll"),
                    new File(@"..\YOY Player\bin\Release\Newtonsoft.Json.dll"),
                    new File(@"..\YOY Player\bin\Release\NAudio.dll"),
                    new File(@"..\YOY Player\bin\Release\Hardcodet.Wpf.TaskbarNotification.dll"),
                    new Dir("es-ES",
                        new File(@"..\YOY Player\bin\Release\es-ES\YOYPlayer.resources.dll"))
                    ),

                new Dir(path),

                new ManagedAction(CustomActions.RunApp,
                    Return.check,
                    When.After,
                    Step.InstallFinalize,
                    Condition.NOT_Installed),

                new ManagedAction(CustomActions.RemoveSettingsFolder,
                    Return.check,
                    When.Before,
                    Step.InstallFinalize,
                    Condition.NOT_Installed)
                );

            project.ControlPanelInfo.ProductIcon = "img-logo.ico";
            project.ControlPanelInfo.Manufacturer = "YOY Player";
            project.Version = new Version(1, 0, 0, 0);

            project.ProductId = new Guid("7a0d1732-916f-4f91-a7f9-78f07cba2c7e");
            project.UpgradeCode = new Guid("451cb091-d221-4c29-beaf-65c0590bf90c");
            project.GUID = new Guid("307b44a9-af65-4466-8ba6-9ea5c1400390");

            project.UI = WUI.WixUI_InstallDir;

            project.SourceBaseDir = Environment.CurrentDirectory;
            project.OutFileName = "YoYPlayer";

            project.CustomUI = new DialogSequence()
                .On(NativeDialogs.WelcomeDlg, Buttons.Next, new ShowDialog(NativeDialogs.InstallDirDlg))
                .On(NativeDialogs.InstallDirDlg, Buttons.Back, new ShowDialog(NativeDialogs.WelcomeDlg));


            //project.WixSourceGenerated += Compiler_WixSourceGenerated;
            Compiler.BuildMsi(project);
        }

        static void Compiler_WixSourceGenerated(XDocument document)
        {
            document.Root.Select("Product")
                         .Add(XElement.Parse(
                                @"<UI>
                                  <Publish Dialog=""WelcomeDlg"" Control=""Next"" Event=""NewDialog"" Order=""5"" Value=""InstallDirDlg"">1</Publish>
                                  <Publish Dialog=""InstallDirDlg"" Control=""Back"" Event=""NewDialog"" Order=""5""  Value=""WelcomeDlg"">1</Publish>
                              </UI>"));
        }
    }

    public class CustomActions
    {
        [CustomAction]
        public static ActionResult RunApp(Session session)
        {
            try
            {
                System.Diagnostics.Process.Start(session["INSTALLDIR"] + @"\YOYPlayer.exe", "INSTALLER");
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return ActionResult.Failure;
            }
        }
        [CustomAction]
        public static ActionResult RemoveSettingsFolder(Session session)
        {
            try
            {
                var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YOYPlayer");

                if (System.IO.Directory.Exists(path))
                    System.IO.Directory.Delete(path, true);
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
                return ActionResult.Success;
            }
        }
    }
}
