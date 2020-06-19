using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOYPlayer.Model.Helpers
{
    public static class FoldersHelper
    {
        public static string PathToAppData { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YOYPlayer");

        public static string PathToAudio { get; } = Path.Combine(PathToAppData, "Audio");
        public static string PathToSettings { get; } = Path.Combine(PathToAppData, "settings");
        public static string PathToErrorsLogs { get; } = Path.Combine(PathToAppData, "errors");
    }
}