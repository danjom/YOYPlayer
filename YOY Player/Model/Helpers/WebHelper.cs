using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YOYPlayer.Model.Data;
using YOYPlayer.Model.Mapping;
using YOYPlayer.Model.Utils.Enums;
using YOYPlayer.Model.Utils.Exceptions;

namespace YOYPlayer.Model.Helpers
{
    public static class WebHelper
    {
        private static string _authURL = "https://data-mng-yoy.azurewebsites.net/token";
        private static string _getCommercesURL = "https://data-mng-yoy.azurewebsites.net/api/Access/Gets?id=";
        private static string _getFileURL = "https://data-mng-yoy.azurewebsites.net/api/File/Get?id={0}&key={1}&branchId={2}&dptId={3}";
        private static string _loadFileURL = "https://data-mng-yoy.azurewebsites.net/api/ContentStream/Get?userId={0}&key={1}&id={2}&downloadType=1";
        private static string _postLogsURL = "https://data-mng-yoy.azurewebsites.net/api/PlayerLog/Post";

        public static async Task<AccessTokenMap> GetNewOAuthToken(string username, string pass)
        {
            var content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "grant_type",  "password" },
                { "username",  username },
                { "password",  pass },
            });

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(_authURL, content);
                if (response.IsSuccessStatusCode)
                {
                    var map = JsonConvert.DeserializeObject<AccessTokenMap>(await response.Content.ReadAsStringAsync());

                    return map;
                }
                else
                    throw new WrongAuthDataException();
            }

        }

        public static async Task<List<Commerce>> GetCommerces()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.OAuthToken) || DateTime.Now > YOYPlayer.Properties.Settings.Default.OAuthExpired)
                throw new AuthTokenExpiredException();


            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Version", "1");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Properties.Settings.Default.OAuthToken}");

                var response = await client.GetStringAsync(_getCommercesURL + Properties.Settings.Default.Username);

                var map = JsonConvert.DeserializeObject<CommercesListMap>(response);

                return map.Commerces;
            }

        }

        public static async Task<AudioFileMap> GetAudioInfo(Commerce commerce, Branche branche, Department dep)
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.OAuthToken) || DateTime.Now > YOYPlayer.Properties.Settings.Default.OAuthExpired)
                throw new AuthTokenExpiredException();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Version", "1");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Properties.Settings.Default.OAuthToken}");

                var response = await client.GetStringAsync(string.Format(_getFileURL, Properties.Settings.Default.Username, branche.AccessKey, branche.ID, dep.ID));

                return JsonConvert.DeserializeObject<AudioFileMap>(response);
            }
        }

        public static async Task LoadAudio(AudioFileMap audioInfo, string accessKey)
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.OAuthToken) || DateTime.Now > YOYPlayer.Properties.Settings.Default.OAuthExpired)
                throw new AuthTokenExpiredException();

            if (!Directory.Exists(FoldersHelper.PathToAudio))
                Directory.CreateDirectory(FoldersHelper.PathToAudio);

            var request = new HttpRequestMessage(HttpMethod.Get, string.Format(_loadFileURL, Properties.Settings.Default.Username, accessKey, audioInfo.FileId));

            using (var client = new WebClient())
            {
                client.Headers.Add("Authorization", $"Bearer {Properties.Settings.Default.OAuthToken}");
                client.Proxy = WebRequest.DefaultWebProxy;

                await client.DownloadFileTaskAsync(string.Format(_loadFileURL, Properties.Settings.Default.Username, accessKey, audioInfo.FileId), Path.Combine(FoldersHelper.PathToAudio, audioInfo.FileName));
            }
        }

        public static async Task SendLogs()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.OAuthToken) || DateTime.Now > YOYPlayer.Properties.Settings.Default.OAuthExpired)
                throw new AuthTokenExpiredException();

            var count = LogsService.Instance.AllLogs.Count(x => x.Sychronized == LogState.UnSync);

            foreach (var x in LogsService.Instance.AllLogs.Where(x => x.Sychronized == LogState.UnSync))
                x.Sychronized = LogState.InSync;

            try
            {
                var json = JsonConvert.SerializeObject(new LogsList(LogsService.Instance.AllLogs.Where(x => x.Sychronized == LogState.InSync)));
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Version", "1");
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Properties.Settings.Default.OAuthToken}");

                    await client.PostAsync(_postLogsURL, content);
                }

                foreach (var x in LogsService.Instance.AllLogs.Where(x => x.Sychronized == LogState.InSync))
                    x.Sychronized = LogState.Synced;
                LogsService.SaveAllLogs();
            }
            catch (Exception ex)
            {
                foreach (var x in LogsService.Instance.AllLogs.Where(x => x.Sychronized == LogState.InSync))
                    x.Sychronized = LogState.UnSync;
            }
        }
    }
}
