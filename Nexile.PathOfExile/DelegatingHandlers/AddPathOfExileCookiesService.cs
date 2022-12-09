using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Nexile.PathOfExile;

namespace Nexile.Infrastructure.Services.PathOfExile.DelegatingHandlers;

public class AddPathOfExileCookiesService : DelegatingHandler
{
    private readonly IPoeSessionIdProvider _sessionIdProvider;

    /// <inheritdoc />
    public AddPathOfExileCookiesService(IPoeSessionIdProvider sessionIdProvider)
    {
        _sessionIdProvider = sessionIdProvider;
    }

    /// <inheritdoc />
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                           CancellationToken cancellationToken)
    {
        if (_sessionIdProvider.SessionId.IsSome)
        {
            request.AddCookie(new KeyValuePair<string, string>("POESESSID", _sessionIdProvider.SessionId.Value));
        }

        return base.SendAsync(request, cancellationToken);
    }
}