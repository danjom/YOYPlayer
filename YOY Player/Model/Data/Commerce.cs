using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOYPlayer.Model.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Commerce
    {
        [JsonProperty(PropertyName = "id")]
        public string ID { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "logo")]
        public string Logo { get; set; }

        [JsonProperty(PropertyName = "branches")]
        public List<Branche> Branches { get; set; }
        [JsonProperty(PropertyName = "departments")]
        public List<Department> Departments { get; set; }
    }
}
