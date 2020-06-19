using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOYPlayer.Model.Data
{
    public class LogsList
    {
        [JsonProperty(PropertyName = "Logs")]
        public List<LogItem> Logs { get; set; }
        [JsonProperty(PropertyName = "Count")]
        public int Count { get; set; }
        [JsonProperty(PropertyName = "From")]
        public DateTime From { get; set; }
        [JsonProperty(PropertyName = "To")]
        public DateTime To { get; set; }

        public  LogsList(IEnumerable<LocalLogItem> list)
        {
            Logs = list.Select(x => x.Convert()).ToList();

            Count = Logs.Count;
            From = Logs.Min(x => x.EventDate);
            To = Logs.Max(x => x.EventDate);
        }
    }
}
