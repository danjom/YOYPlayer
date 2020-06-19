using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOYPlayer.Model.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Branche
    {
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "accessKey")]
        public string AccessKey { get; set; }
    }
}
