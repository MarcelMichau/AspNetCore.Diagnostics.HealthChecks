using Azure.Core;
using HealthChecks.AzureAppConfiguration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods to configure <see cref="AzureAppConfigurationHealthCheck"/>.
    /// </summary>
    public static class AzureAppConfigurationHealthChecksBuilderExtensions
    {
        private const string APPCONFIGURATION_NAME = "azureappconfiguration";

        /// <summary>
        /// Add a health check for Azure App Configuration. Default behaviour is using Managed Service Identity, to use Client Secrets call UseClientSecrets in setup action
        /// </summary>
        /// <param name="appConfigurationServiceUri">The AzureAppConfiguration service uri.</param>
        /// <param name="credential">The TokenCredential to use, you can use Azure.Identity with DefaultAzureCredential or other kind of TokenCredential,you can read more on <see href="https://github.com/Azure/azure-sdk-for-net/blob/Azure.Identity_1.2.2/sdk/identity/Azure.Identity/README.md"/>. </param>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="setup"> Setup action to configure Azure App Configuration options.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'azureappconfiguration' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="timeout">An optional <see cref="TimeSpan"/> representing the timeout of the check.</param>
        /// <returns>The specified <paramref name="builder"/>.</returns>
        public static IHealthChecksBuilder AddAzureAppConfiguration(
            this IHealthChecksBuilder builder,
            Uri appConfigurationServiceUri,
            TokenCredential credential,
            Action<AzureAppConfigurationOptions>? setup,
            string? name = default,
            HealthStatus? failureStatus = default,
            IEnumerable<string>? tags = default,
            TimeSpan? timeout = default)
        {
            if (appConfigurationServiceUri == null)
            {
                throw new ArgumentNullException(nameof(appConfigurationServiceUri));
            }

            return AddAzureAppConfiguration(builder, _ => appConfigurationServiceUri, credential, (_, options) => setup?.Invoke(options), name, failureStatus, tags, timeout);
        }

        /// <summary>
        /// Add a health check for Azure Key Vault. Default behaviour is using Managed Service Identity, to use Client Secrets call UseClientSecrets in setup action
        /// </summary>
        /// <param name="appConfigurationServiceUri">The AzureAppConfiguration service uri.</param>
        /// <param name="credential">The TokenCredential to use, you can use Azure.Identity with DefaultAzureCredential or other kind of TokenCredential, you can read more on <see href="https://github.com/Azure/azure-sdk-for-net/blob/Azure.Identity_1.2.2/sdk/identity/Azure.Identity/README.md"/>. </param>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="setup"> Setup action to configure Azure Key Vault options.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'azureappconfiguration' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="timeout">An optional <see cref="TimeSpan"/> representing the timeout of the check.</param>
        /// <returns>The specified <paramref name="builder"/>.</returns>
        public static IHealthChecksBuilder AddAzureAppConfiguration(
            this IHealthChecksBuilder builder,
            Uri appConfigurationServiceUri,
            TokenCredential credential,
            Action<IServiceProvider, AzureAppConfigurationOptions> setup,
            string? name = default,
            HealthStatus? failureStatus = default,
            IEnumerable<string>? tags = default,
            TimeSpan? timeout = default)
        {
            if (appConfigurationServiceUri == null)
            {
                throw new ArgumentNullException(nameof(appConfigurationServiceUri));
            }

            return AddAzureAppConfiguration(builder, _ => appConfigurationServiceUri, credential, setup, name, failureStatus, tags, timeout);
        }

        /// <summary>
        /// Add a health check for Azure Key Vault. Default behaviour is using Managed Service Identity, to use Client Secrets call UseClientSecrets in setup action
        /// </summary>
        /// <param name="appConfigurationServiceUriFactory">A factory to build the key vault URI to use.</param>
        /// <param name="credential">The TokenCredential to use, you can use Azure.Identity with DefaultAzureCredential or other kind of TokenCredential, you can read more on <see href="https://github.com/Azure/azure-sdk-for-net/blob/Azure.Identity_1.2.2/sdk/identity/Azure.Identity/README.md"/>. </param>
        /// <param name="builder">The <see cref="IHealthChecksBuilder"/>.</param>
        /// <param name="setup"> Setup action to configure Azure Key Vault options.</param>
        /// <param name="name">The health check name. Optional. If <c>null</c> the type name 'azureappconfiguration' will be used for the name.</param>
        /// <param name="failureStatus">
        /// The <see cref="HealthStatus"/> that should be reported when the health check fails. Optional. If <c>null</c> then
        /// the default status of <see cref="HealthStatus.Unhealthy"/> will be reported.
        /// </param>
        /// <param name="tags">A list of tags that can be used to filter sets of health checks. Optional.</param>
        /// <param name="timeout">An optional <see cref="TimeSpan"/> representing the timeout of the check.</param>
        /// <returns>The specified <paramref name="builder"/>.</returns>
        public static IHealthChecksBuilder AddAzureAppConfiguration(
            this IHealthChecksBuilder builder,
            Func<IServiceProvider, Uri> appConfigurationServiceUriFactory,
            TokenCredential credential,
            Action<IServiceProvider, AzureAppConfigurationOptions>? setup,
            string? name = default,
            HealthStatus? failureStatus = default,
            IEnumerable<string>? tags = default,
            TimeSpan? timeout = default)
        {
            if (appConfigurationServiceUriFactory == null)
                throw new ArgumentNullException(nameof(appConfigurationServiceUriFactory));

            if (credential == null)
                throw new ArgumentNullException(nameof(credential));

            return builder.Add(new HealthCheckRegistration(
               name ?? APPCONFIGURATION_NAME,
               sp =>
               {
                   var options = new AzureAppConfigurationOptions();
                   setup?.Invoke(sp, options);
                   return new AzureAppConfigurationHealthCheck(appConfigurationServiceUriFactory(sp), credential, options);
               },
               failureStatus,
               tags,
               timeout));
        }
    }
}
