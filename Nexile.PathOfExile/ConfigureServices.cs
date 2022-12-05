using Microsoft.Extensions.DependencyInjection;
using Nexile.Common;
using Nexile.Common.DelegatingHandlers;
using Nexile.Common.Interfaces;
using Nexile.Infrastructure.Services.PathOfExile.DelegatingHandlers;
using Nexile.PathOfExile;
using RestEase.HttpClientFactory;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddOfficialPoeApiServices(this IServiceCollection services)
    {
        services.AddSingleton<IPoeSessionIdService, PoeSessionIdService>();
        services.AddSingleton<AddCommonRequestHeadersHandler>();
        services.AddSingleton<RateLimitingTrackerService>();
        
        services.AddTransient<PathOfExileApi>();
        services.AddTransient<AddPathOfExileCookiesService>();
        services.AddTransient<ICommonRequestHeadersProvider, NexileUserAgentHeaderProvider>();
        
        services.AddRestEaseClient<IOfficialTradeRestApi>(PathOfExileApi.PoeWebsiteUrl)
            .AddHttpMessageHandler<AddPathOfExileCookiesService>()
            .AddHttpMessageHandler<AddCommonRequestHeadersHandler>()
            .AddHttpMessageHandler<RateLimitingTrackerService>();
        
        return services;
    }
}