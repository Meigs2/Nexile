using Microsoft.Extensions.DependencyInjection;
using Nexile.PathOfExile;

namespace Nexile.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddNexileInfrastructureServices(this IServiceCollection services, Action<InfrastructureOptions> configureOptions)
    {
        var options = new InfrastructureOptions();
        configureOptions(options);
        return services.AddNexileInfrastructureServicesInternal(options);
    }
    
    private static IServiceCollection AddNexileInfrastructureServicesInternal(this IServiceCollection services, InfrastructureOptions options)
    {
        services.AddSingleton(options);
        services.AddSingleton(options.SessionIdOptions);
        services.AddLogging();
        services.AddOfficialPoeApiServices();
        services.AddPoeNinjaServices();

        return services;
    }
    
    public class InfrastructureOptions
    {
        public string PoeSessionId { get; set; }
        
        public SessionIdOptions SessionIdOptions { get; } = new();

        public string SessionId
        {
            get => SessionIdOptions.SessionId;
            set => SessionIdOptions.SessionId = value;
        }
    }
}