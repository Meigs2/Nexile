using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Nexile.Common.Interfaces;

namespace Nexile.Common.DelegatingHandlers;

public class AddCommonRequestHeadersHandler : DelegatingHandler
{
    private readonly ICommmonHeadersProvider _commmonHeadersProvider;

    public AddCommonRequestHeadersHandler(ICommmonHeadersProvider commmonHeadersProvider)
    {
        _commmonHeadersProvider = commmonHeadersProvider;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                           CancellationToken cancellationToken)
    {
        foreach (var header in _commmonHeadersProvider.Headers)
        {
            request.Headers.Add(header.Key, header.Value);
        }

        return base.SendAsync(request, cancellationToken);
    }
}