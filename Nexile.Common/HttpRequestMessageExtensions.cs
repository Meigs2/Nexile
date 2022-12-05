// ReSharper disable once CheckNamespace

using System.Collections.Generic;

namespace System.Net.Http;

public static class HttpRequestMessageExtensions
{
    public static void AddCookie(this HttpRequestMessage httpRequestMessage, KeyValuePair<string,string> cookie)
    {
        var newCookie = new Cookie(cookie.Key, cookie.Value)
        {
            Domain = httpRequestMessage.RequestUri?.Host
        };
        
        httpRequestMessage.Headers.Add("Cookie", newCookie.ToString());
    }

    public static CookieCollection GetCookie(this HttpRequestMessage httpRequestMessage)
    {
        var requestUri = httpRequestMessage.RequestUri;
        var cookieContainer = new CookieContainer();
        if (!httpRequestMessage.Headers.TryGetValues("Cookie", out var cookieValueList))
            return cookieContainer.GetCookies(requestUri);
        
        foreach (var value in cookieValueList)
        {
            cookieContainer.SetCookies(requestUri, value);
        }

        return cookieContainer.GetCookies(requestUri);
    }
}