namespace HealthChecks.AzureAppConfiguration;

/// <summary>
/// Azure App Configuration configuration options.
/// </summary>
public class AzureAppConfigurationOptions
{
    internal HashSet<string> _configSettings = new();

    internal IEnumerable<string> ConfigSettings => _configSettings;

    /// <summary>
    /// Add a Azure App Configuration config to be checked
    /// </summary>
    /// <param name="configSettingKey">The config setting key to be checked</param>
    /// <returns><see cref="AzureAppConfigurationOptions"/></returns>
    public AzureAppConfigurationOptions AddConfigSetting(string configSettingKey)
    {
        _configSettings.Add(configSettingKey);

        return this;
    }
}
