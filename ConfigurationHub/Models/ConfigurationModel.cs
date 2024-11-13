using System.Collections.Generic;

namespace ConfigurationHub.Models
{
    public class ConfigurationModel
    {
        public string ApplicationName { get; set; }
        
        public List<ConfigurationItemModel> Items { get; set; } = new();
    }
}