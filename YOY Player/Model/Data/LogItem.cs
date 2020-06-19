using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOYPlayer.Model.Data
{
    public class LogItem
    {
        [JsonProperty(PropertyName = "FileId")]
        public string FileID { get; set; }

        [JsonProperty(PropertyName = "BranchId")]
        public string BranchId { get; set; } = "Branche";
        [JsonProperty(PropertyName = "DepartmentId")]
        public string DepartmentId { get; set; } = "Dep";

        [JsonProperty(PropertyName = "Username")]
        public string Username { get; set; } = "User";
        [JsonProperty(PropertyName = "AccessKey")]
        public string AccessKey { get; set; }

        [JsonProperty(PropertyName = "EventDate")]
        public DateTime EventDate { get; set; }
        [JsonProperty(PropertyName = "EventType")]
        public int EventType { get; set; } = 1;
    }
}
