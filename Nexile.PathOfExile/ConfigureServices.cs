using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Nexile.Common;
using Nexile.Common.DelegatingHandlers;
using Nexile.Common.Interfaces;
using Nexile.Infrastructure.Services.PathOfExile.DelegatingHandlers;
using Nexile.PathOfExile;
using OAuth2ClientHandler;
using RestEase.HttpClientFactory;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddOfficialPoeApiServices(this IServiceCollection services)
    {
        services.AddSingleton<ISessionIdProvider, SessionIdProvider>();
        services.AddSingleton<AddCommonRequestHeadersHandler>();
        services.AddSingleton<RateLimitingTrackerService>();
        services.AddTransient<PathOfExileApi>();
        services.AddTransient<AddPathOfExileCookiesService>();
        services.AddTransient<ICommmonHeadersProvider, NexileUserAgentHeaderProvider>();
        services.AddRestEaseClient<IOfficialTradeRestApi>(IPathOfExileApi.TradeApiBaseUrl)
                .AddHttpMessageHandler<AddPathOfExileCookiesService>()
                .AddHttpMessageHandler<AddCommonRequestHeadersHandler>()
                .AddHttpMessageHandler<RateLimitingTrackerService>();

        //services.AddRestEaseClient<IPathOfExileApi>(IPathOfExileApi.OAuthApiBaseUrl);
        return services;
    }
}