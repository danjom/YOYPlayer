using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YOYPlayer.Model.Data;
using YOYPlayer.Model.Helpers;

namespace YOYPlayer.Model
{
    public class LogsService : INotifyPropertyChanged
    {
        private static string _logsFile = Path.Combine(FoldersHelper.PathToAppData, "logs.txt");
        private static object _locker = new object();

        #region Properties

        public List<LocalLogItem> AllLogs { get; set; } = new List<LocalLogItem>();

        #endregion

        #region Singleton

        private static LogsService _instance = null;

        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public static LogsService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LogsService();
                    _instance.AllLogs = ReadAllLogs();
                }

                return _instance;
            }
        }

        #endregion

        public static void WriteLog(LocalLogItem newItem)
        {
            lock (_locker)
            {
                Instance.AllLogs.Insert(0, newItem);
                SaveAllLogs();

                Instance.Notify("AllLogs");
            }
        }

        public static List<LocalLogItem> ReadAllLogs()
        {
            var res = new List<LocalLogItem>();

            lock (_locker)
            {
                CheckFile();

                using (var reader = new StreamReader(_logsFile))
                    res = JsonConvert.DeserializeObject<List<LocalLogItem>>(reader.ReadToEnd());
            }

            return res?.Where(l => DateTime.Now.Month - l.EventDate.Month < 4).OrderByDescending(x => x.EventDate).ToList() ?? new List<LocalLogItem>();
        }

        public static void SaveAllLogs()
        {
            lock (_locker)
            {
                CheckFile();

                using (var writer = new StreamWriter(_logsFile, false))
                    writer.Write(JsonConvert.SerializeObject(_instance.AllLogs));
            }
        }

        private static void CheckFile()
        {
            if (!Directory.Exists(FoldersHelper.PathToAppData))
                Directory.CreateDirectory(FoldersHelper.PathToAppData);

            if (!File.Exists(_logsFile))
                File.Create(_logsFile).Close();
        }
    }
}
