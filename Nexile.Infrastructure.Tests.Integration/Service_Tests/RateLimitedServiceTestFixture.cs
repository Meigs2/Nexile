using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nexile.Infrastructure.Tests.Integration.Service_Tests;

[TestFixture]
public abstract class RateLimitedServiceTestFixture
{
    protected static ServiceProvider ServiceProvider { get; private set; }

    public static class Secrets
    {
        private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
                                                               // add user secrets
                                                               .AddUserSecrets(typeof(Secrets).Assembly)
                                                               .Build();

        public static string ProvidedSessionId => Configuration["PathOfExileApiSessionId"] ?? throw new Exception("A session ID must be set to run Nexile's infrastructure tests. Please see the README.md for more information.");
    }

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        var services = new ServiceCollection();
        services.AddNexileInfrastructureServices(options => { options.SessionId = Secrets.ProvidedSessionId; });
        ServiceProvider = services.BuildServiceProvider();
    }

    [TearDown]
    public void TeardownService()
    {
        // Add sleep after each service test to ensure we dont spam anything too hard while testing.
        Thread.Sleep(1000);
    }
}