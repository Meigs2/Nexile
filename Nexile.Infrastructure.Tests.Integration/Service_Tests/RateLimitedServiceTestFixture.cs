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

        public static string ProvidedSessionId => Configuration["PathOfExileApiSessionId"];
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