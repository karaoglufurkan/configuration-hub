using ConfigurationHub.Providers;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigurationHub.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurationReader(this IServiceCollection services, string connectionString, string applicationName, long refreshIntervalInMilliseconds)
        {
            services.AddSingleton<IConfigurationReader, ConfigurationReader>(provider => new ConfigurationReader(connectionString, applicationName, refreshIntervalInMilliseconds));
            
            return services;
        }
    }
}