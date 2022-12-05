using Nexile.Common.DelegatingHandlers;
using Nexile.PoeNinja.Core;
using RestEase.HttpClientFactory;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices
{
    public static IServiceCollection AddPoeNinjaServices(this IServiceCollection services)
    {
        services.AddRestEaseClient<ICurrencyOverview>("https://poe.ninja")
            .AddHttpMessageHandler<AddCommonRequestHeadersHandler>();
        
        return services;
    }
}