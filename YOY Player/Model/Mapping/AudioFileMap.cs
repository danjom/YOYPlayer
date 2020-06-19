using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOYPlayer.Model.Mapping
{
    [JsonObject(MemberSerialization.OptIn)]
    public class AudioFileMap
    {
        [JsonProperty(PropertyName = "mimeType")]
        public string MimeType { get; set; }
        [JsonProperty(PropertyName = "fileName")]
        public string FileName { get; set; }
        [JsonProperty(PropertyName = "fileId")]
        public string FileId { get; set; }
        [JsonProperty(PropertyName = "nextSync")]
        public string NextSync { get; set; }
        [JsonProperty(PropertyName = "requestDate")]
        public string RequestDate { get; set; }
    }
}
