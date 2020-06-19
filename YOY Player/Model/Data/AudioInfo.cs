using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOYPlayer.Model.Data
{
    [JsonObject(MemberSerialization.OptIn)]
    [TypeConverter(typeof(AudioInfoConverter))]
    [SettingsSerializeAs(SettingsSerializeAs.String)]
    public class AudioInfo
    {
        [JsonProperty(PropertyName = "fileID")]
        public string FileID { get; set; }

        [JsonProperty(PropertyName = "accessKey")]
        public string AccessKey { get; set; }

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
    }

    public class AudioInfoConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            else
                return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            string stringValue = value as string;
            if (stringValue != null)
            {
                return JsonConvert.DeserializeObject<AudioInfo>(stringValue);
            }
            else
                return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            else
                return base.CanConvertFrom(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
                return JsonConvert.SerializeObject(value);
            else
                return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
