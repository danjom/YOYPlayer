using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using YOYPlayer.Model.Helpers;

namespace YOYPlayer.Model.Utils
{
    [RunInstaller(true)]
    public partial class UninstallFolder : System.Configuration.Install.Installer
    {
        public UninstallFolder()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            try
            {
                if (Directory.Exists(FoldersHelper.PathToAppData))
                    Directory.Delete(FoldersHelper.PathToAppData, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Install Method");
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);

            try
            {
                if (Directory.Exists(FoldersHelper.PathToAppData))
                    Directory.Delete(FoldersHelper.PathToAppData, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Uninstall Method");
            }
        }
    }
}