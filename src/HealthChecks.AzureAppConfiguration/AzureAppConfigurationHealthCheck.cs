using System.Collections.Concurrent;
using Azure.Core;
using Azure.Data.AppConfiguration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecks.AzureAppConfiguration
{
    public class AzureAppConfigurationHealthCheck : IHealthCheck
    {
        private readonly AzureAppConfigurationOptions _options;
        private readonly Uri _appConfigurationUri;
        private readonly TokenCredential _azureCredential;

        private static readonly ConcurrentDictionary<Uri, ConfigurationClient> _configurationClientsHolder = new ConcurrentDictionary<Uri, ConfigurationClient>();

        public AzureAppConfigurationHealthCheck(Uri appConfigurationUri, TokenCredential azureCredential, AzureAppConfigurationOptions options)
        {
            _appConfigurationUri = appConfigurationUri ?? throw new ArgumentNullException(nameof(appConfigurationUri));
            _azureCredential = azureCredential ?? throw new ArgumentNullException(nameof(azureCredential));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {

                foreach (var configSetting in _options.ConfigSettings)
                {
                    var configurationClient = CreateConfigurationClient();
                    await configurationClient.GetConfigurationSettingAsync(configSetting, cancellationToken: cancellationToken);
                }

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }

        private ConfigurationClient CreateConfigurationClient()
        {
            if (!_configurationClientsHolder.TryGetValue(_appConfigurationUri, out var client))
            {
                client = new ConfigurationClient(_appConfigurationUri, _azureCredential);
                _configurationClientsHolder.TryAdd(_appConfigurationUri, client);
            }

            return client;
        }
    }
}
