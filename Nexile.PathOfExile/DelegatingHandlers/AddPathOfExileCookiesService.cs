using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Nexile.PathOfExile;

namespace Nexile.Infrastructure.Services.PathOfExile.DelegatingHandlers;

public class AddPathOfExileCookiesService : DelegatingHandler
{
    private readonly IPoeSessionIdService _sessionIdService;

    /// <inheritdoc />
    public AddPathOfExileCookiesService(IPoeSessionIdService sessionIdService)
    {
        _sessionIdService = sessionIdService;
    }

    /// <inheritdoc />
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                           CancellationToken cancellationToken)
    {
        if (_sessionIdService.SessionId.IsSome)
        {
            request.AddCookie(new KeyValuePair<string, string>("POESESSID", _sessionIdService.SessionId.Value));
        }

        return base.SendAsync(request, cancellationToken);
    }
}