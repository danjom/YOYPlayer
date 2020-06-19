using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YOYPlayer.Model.Data;

namespace YOYPlayer.Model.Mapping
{
    [JsonObject(MemberSerialization.OptIn)]
    public class CommercesListMap
    {
        [JsonProperty(PropertyName = "username")]
        public string Username { get; set; }
        [JsonProperty(PropertyName = "commerces")]
        public List<Commerce> Commerces { get; set; }
    }
}
