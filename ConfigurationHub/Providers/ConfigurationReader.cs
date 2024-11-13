using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ConfigurationHub.Helpers;
using ConfigurationHub.Models;
using StackExchange.Redis;

namespace ConfigurationHub.Providers
{
    public class ConfigurationReader : IConfigurationReader
    {
        private const string SetLockKey = "set-lock-key";
        private bool _isInitialized;
        private ConfigurationModel _configuration = new();
        
        private readonly long _refreshIntervalInMilliseconds;
        private readonly string _applicationName;
        private readonly IDatabase _database;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public ConfigurationReader(string connectionString, string applicationName, long refreshIntervalInMilliseconds)
        {
            _applicationName = applicationName;
            _refreshIntervalInMilliseconds = refreshIntervalInMilliseconds;
            
            var connection = ConnectionMultiplexer.Connect(connectionString);
            _database = connection.GetDatabase();
            _cancellationTokenSource = new CancellationTokenSource();
            
            Initialize();
        }

        public T GetValue<T>(string key)
        {
            ConfigurationItemModel configurationItem = _configuration.Items.FirstOrDefault(x => x.Key == key && x.IsActive);
            
            if (configurationItem?.Value == null)
            {
                return default;
            }

            if (TypeConversionHelper.IsConversionPossible(configurationItem.Value, typeof(T)) is false)
            {
                throw new InvalidOperationException($"The value for key '{key}' cannot be converted to type {typeof(T).Name}.");
            }
            
            return (T)Convert.ChangeType(configurationItem.Value, typeof(T));
        }

        public List<ConfigurationItemModel> GetApplicationConfigurations()
        {
            return _configuration.Items.Where(x => x.IsActive).ToList();
        }

        public async Task SetValue(string key, ConfigurationItemModel newItem)
        {
            Type targetType = TypeConversionHelper.GetTypeFromEnum(newItem.Type);
            
            if (TypeConversionHelper.IsConversionPossible(newItem.Value, targetType) is false)
            {
                throw new InvalidOperationException($"The value for key '{key}' is invalid for type {targetType.Name}.");
            }
            
            if (TypeConversionHelper.AcceptedTypes.Contains(targetType) is false)
            {
                throw new InvalidOperationException($"The type {targetType.Name} is not supported.");
            }

            bool lockAcquired = await AcquireLock();

            if (lockAcquired)
            {
                try
                {
                    string serializedConfiguration = await _database.StringGetAsync(_applicationName);

                    ConfigurationModel configuration;

                    if (serializedConfiguration == null)
                    {
                        configuration = new ConfigurationModel
                        {
                            ApplicationName = _applicationName
                        };
                    }
                    else
                    {
                        configuration = JsonSerializer.Deserialize<ConfigurationModel>(serializedConfiguration);
                    }
            
                    var existingItem = configuration.Items.FirstOrDefault(x => x.Key == key && x.IsActive);

                    if (existingItem == null)
                    {
                        configuration.Items.Add(new ConfigurationItemModel
                        {
                            Key = key,
                            Value = newItem.Value,
                            Type = newItem.Type,
                            IsActive = true
                        });
                    }
                    else
                    {
                        existingItem.Value = newItem.Value;
                        existingItem.Type = newItem.Type;
                    }
            
                    serializedConfiguration = JsonSerializer.Serialize(configuration);
            
                    await _database.StringSetAsync(_applicationName, serializedConfiguration);
                }
                finally
                {
                    await ReleaseLock();
                }
            }
            else
            {
                throw new InvalidOperationException("Unable to acquire lock!");
            }
        }

        public async Task DeleteValue(string key)
        {
            bool lockAcquired = await AcquireLock();

            if (lockAcquired)
            {
                try
                {
                    string serializedConfiguration = await _database.StringGetAsync(_applicationName);

                    ConfigurationModel configuration = JsonSerializer.Deserialize<ConfigurationModel>(serializedConfiguration);

                    var item = configuration.Items.FirstOrDefault(x => x.Key == key && x.IsActive);

                    if (item == null)
                    {
                        return;
                    }

                    item.IsActive = false;

                    serializedConfiguration = JsonSerializer.Serialize(configuration);

                    await _database.StringSetAsync(_applicationName, serializedConfiguration);
                }
                finally
                {
                    await ReleaseLock();
                }
            }
            else
            {
                throw new InvalidOperationException("Unable to acquire lock!");
            }
        }
        
        private void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            _isInitialized = true;
            
            _ = Task.Run(async () =>
            {
                while (_cancellationTokenSource.IsCancellationRequested == false)
                {
                    try
                    {
                        await Refresh();

                        await Task.Delay(TimeSpan.FromMilliseconds(_refreshIntervalInMilliseconds), _cancellationTokenSource.Token);
                    }
                    catch (TaskCanceledException)
                    {
                        //ignore
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            });
        }

        private async Task Refresh()
        {
            string serializedConfiguration = await _database.StringGetAsync(_applicationName);

            _configuration = JsonSerializer.Deserialize<ConfigurationModel>(serializedConfiguration);
        }

        private async Task<bool> AcquireLock()
        {
            return await _database.StringSetAsync(SetLockKey, "locked", TimeSpan.FromSeconds(10), When.NotExists);
        }

        private async Task ReleaseLock()
        {
            await _database.KeyDeleteAsync(SetLockKey);
        }
    }
}