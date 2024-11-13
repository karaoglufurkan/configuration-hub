using System.Collections.Generic;
using System.Threading.Tasks;
using ConfigurationHub.Models;

namespace ConfigurationHub.Providers
{
    public interface IConfigurationReader
    {
        T GetValue<T>(string key);
        
        List<ConfigurationItemModel> GetApplicationConfigurations();
        
        Task SetValue(string key, ConfigurationItemModel value);
        
        Task DeleteValue(string key);
    }
}