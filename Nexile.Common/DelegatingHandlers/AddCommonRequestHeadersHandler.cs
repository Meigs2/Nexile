using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Nexile.Common.Interfaces;

namespace Nexile.Common.DelegatingHandlers;

public class AddCommonRequestHeadersHandler : DelegatingHandler
{
    private readonly ICommonRequestHeadersProvider _commonRequestHeadersProvider;

    public AddCommonRequestHeadersHandler(ICommonRequestHeadersProvider commonRequestHeadersProvider)
    {
        _commonRequestHeadersProvider = commonRequestHeadersProvider;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                           CancellationToken cancellationToken)
    {
        foreach (var header in _commonRequestHeadersProvider.Headers)
        {
            request.Headers.Add(header.Key, header.Value);
        }

        return base.SendAsync(request, cancellationToken);
    }
}