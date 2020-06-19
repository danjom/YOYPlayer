using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YOYPlayer.Model.Utils.Enums;

namespace YOYPlayer.Model.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LocalLogItem
    {
        [JsonProperty(PropertyName = "fileID")]
        public string FileID { get; set; }

        [JsonProperty(PropertyName = "commerceName")]
        public string CommerceName { get; set; } = "Comm";
        [JsonProperty(PropertyName = "branchName")]
        public string BranchName { get; set; } = "Branche";
        [JsonProperty(PropertyName = "departmentName")]
        public string DepartmentName { get; set; } = "Dep";

        [JsonProperty(PropertyName = "commerceId")]
        public string CommerceId { get; set; } = "Comm";
        [JsonProperty(PropertyName = "branchId")]
        public string BranchId { get; set; } = "Branche";
        [JsonProperty(PropertyName = "departmentId")]
        public string DepartmentId { get; set; } = "Dep";

        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; } = "User";
        [JsonProperty(PropertyName = "accessKey")]
        public string AccessKey { get; set; }

        [JsonProperty(PropertyName = "eventDate")]
        public DateTime EventDate { get; set; }
        [JsonProperty(PropertyName = "eventType")]
        public int EventType { get; set; } = 1;

        [JsonProperty(PropertyName = "sync")]
        public LogState Sychronized { get; set; } = LogState.UnSync;

        public LogItem Convert()
        {
            return new LogItem()
            {
                FileID = FileID,
                AccessKey = AccessKey,
                BranchId = BranchId,
                DepartmentId = DepartmentId,
                EventDate = EventDate,
                EventType = EventType,
                Username = Username,
            };
        }
    }
}
