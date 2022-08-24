using Azure.Core;

namespace HealthChecks.AzureAppConfiguration.Tests.DependencyInjection
{
    public class azure_appconfiguration_registration_should
    {
        [Fact]
        public void add_health_check_when_properly_configured()
        {
            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddAzureAppConfiguration(new Uri("http://localhost"), new MockTokenCredentials(), setup =>
                {
                    setup
                        .AddConfigSetting("configkey");
                });

            using var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            registration.Name.Should().Be("azureappconfiguration");
            check.GetType().Should().Be(typeof(AzureAppConfigurationHealthCheck));

        }

        [Fact]
        public void add_named_health_check_when_properly_configured()
        {
            var services = new ServiceCollection();
            services.AddHealthChecks()
                .AddAzureAppConfiguration(new Uri("http://localhost"), new MockTokenCredentials(), options => { }, name: "appconfigurationcheck");

            using var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            registration.Name.Should().Be("appconfigurationcheck");
            check.GetType().Should().Be(typeof(AzureAppConfigurationHealthCheck));
        }

        [Fact]
        public void fail_when_invalid_uri_provided_in_configuration()
        {
            var services = new ServiceCollection();

            Assert.Throws<ArgumentNullException>(() =>
            {
                services.AddHealthChecks()
                .AddAzureAppConfiguration(null!, new MockTokenCredentials(), setup =>
                 {
                     setup
                         .AddConfigSetting("configkey");
                 });
            });
        }

        [Fact]
        public void fail_when_invalid_uri_provided_with_service_provider_based_setup_in_configuration()
        {
            var services = new ServiceCollection();

            Assert.Throws<ArgumentNullException>(() =>
            {
                services.AddHealthChecks()
                .AddAzureAppConfiguration((Uri)null!, new MockTokenCredentials(), (_, setup) =>
                {
                    setup
                        .AddConfigSetting("configkey");
                });
            });
        }

        [Fact]
        public void fail_when_invalid_credential_provided_in_configuration()
        {
            var services = new ServiceCollection();

            Assert.Throws<ArgumentNullException>(() =>
            {
                services.AddHealthChecks()
                    .AddAzureAppConfiguration(new Uri("http://localhost"), null!, setup =>
                    {
                        setup
                            .AddConfigSetting("configkey");
                    });
            });
        }

        [Fact]
        public void add_health_check_with_appconfiguration_service_uri_factory_when_properly_configured()
        {
            var services = new ServiceCollection();
            var factoryCalled = false;
            var setupCalled = false;

            services.AddHealthChecks()
                .AddAzureAppConfiguration(
                    _ =>
                    {
                        factoryCalled = true;
                        return new Uri("http://localhost");
                    },
                    new MockTokenCredentials(),
                    (_, _) => setupCalled = true);

            using var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            check.GetType().Should().Be(typeof(AzureAppConfigurationHealthCheck));
            factoryCalled.Should().BeTrue();
            setupCalled.Should().BeTrue();
        }

        [Fact]
        public void add_health_check_with_service_provider_based_setup_when_properly_configured()
        {
            var services = new ServiceCollection();
            var setupCalled = false;

            services.AddHealthChecks()
                .AddAzureAppConfiguration(
                    new Uri("http://localhost"),
                    new MockTokenCredentials(),
                    (_, _) => setupCalled = true);

            using var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<HealthCheckServiceOptions>>();

            var registration = options.Value.Registrations.First();
            var check = registration.Factory(serviceProvider);

            check.GetType().Should().Be(typeof(AzureAppConfigurationHealthCheck));
            setupCalled.Should().BeTrue();
        }
    }

    public class MockTokenCredentials : TokenCredential
    {
        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
