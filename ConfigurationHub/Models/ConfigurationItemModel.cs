using System.Text.Json.Serialization;
using ConfigurationHub.Enums;

namespace ConfigurationHub.Models
{
    public class ConfigurationItemModel
    {
        public string Key { get; set; }

        public string Value { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ConfigurationValueTypes Type { get; set; }

        public bool IsActive { get; set; }
    }
}