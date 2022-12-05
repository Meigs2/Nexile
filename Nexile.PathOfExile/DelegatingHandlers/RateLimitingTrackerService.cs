using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Nexile.PathOfExile.Attributes;

namespace Nexile.Infrastructure.Services.PathOfExile.DelegatingHandlers;

public class RateLimitingTrackerService : DelegatingHandler
{
    private IRequestRateLimiter _rateLimitService;
    
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                                                           CancellationToken cancellationToken)
    {
        var a = new StackTrace(true);
        var method = (a.GetFrames())
                      .FirstOrDefault(x => x?.GetMethod()?.GetCustomAttributes<RateLimitedAttribute>() is not null)
                      ?.GetMethod();
        
        if (method is not null)
        {
            
        }
        
        var result = base.SendAsync(request, cancellationToken);
        // update rate limiting results 
        return result;
    }

    private void ProcessHttpMessage(HttpResponseMessage response)
    {
    }
}

public interface IRequestRateLimiter
{
    
}