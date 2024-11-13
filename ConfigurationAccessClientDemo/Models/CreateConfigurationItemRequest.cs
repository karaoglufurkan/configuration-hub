using System.Text.Json.Serialization;
using ConfigurationHub.Enums;

namespace DemoClient.Models
{
    public class CreateConfigurationItemRequest
    {
        public string Key { get; set; }

        public string Value { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ConfigurationValueTypes Type { get; set; }
    }
}