using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Nexile.Common.Interfaces;
using RestEase;
using Websocket.Client;
using Meigs2.Functional;
using Meigs2.Functional.Common;
using Meigs2.Functional.Results;

namespace Nexile.PathOfExile;

public class PathOfExileApi : IPathOfExileApi
{
    private readonly IOfficialTradeRestApi _restApi;
    private readonly IPoeSessionIdProvider _sessionIdProvider;
    private readonly ICommonRequestHeadersProvider _commonRequestHeadersProvider;
    public const string PoeLiveSearchUrl = "wss://www.pathofexile.com";
    public const string PoeWebsiteUrl = "https://www.pathofexile.com";
    private ConcurrentDictionary<TradeSearch, LiveSearch> _subscriptions = new();
    public List<LiveSearch> LiveSearches => _subscriptions.Values.ToList();

    public PathOfExileApi(IOfficialTradeRestApi restApi,
        IPoeSessionIdProvider sessionIdProvider,
        ICommonRequestHeadersProvider commonRequestHeadersProvider)
    {
        _restApi = restApi;
        _sessionIdProvider = sessionIdProvider;
        _commonRequestHeadersProvider = commonRequestHeadersProvider;
    }

    public async Task<Result<ExchangePostResult>> SearchExchange(ExchangeQuery query)
    {
        try
        {
            var response = await _restApi.PostExchange(query.QueryString, query.LeagueName);
            return GetResponseContent(response);
        }
        catch (Exception e) { return e; }
    }

    public async Task<Result<TradeSearch>> CreateItemSearch(ItemSearchQuery query)
    {
        try
        {
            // remove all the query string's whitespace characters
            var response = await _restApi.PostSearch(query.QueryString, query.LeagueName);
            var itemSearchResult = GetResponseContent(response);
            if (itemSearchResult.IsFailure) return Result.Failure(itemSearchResult.Errors);
            var itemSearch = itemSearchResult.Value;
            return new TradeSearch
            {
                OriginalQuery = query,
                QueryId = itemSearch.QueryId,
                Complexity = itemSearch.Complexity,
                NumberOfResults = itemSearch.Total,
                ListingIds = itemSearch.Results
            };
        }
        catch (Exception e) { return e; }
    }

    public async Task<Result<SearchGetResult>> GetItemSearchResults(TradeSearch search, int numberOfResults = 10)
    {
        try
        {
            var result = new SearchGetResult(new List<SearchResult>());
            for (var i = 0; i < numberOfResults; i += 10)
            {
                var response =
                    await _restApi.GetSearch(string.Join(",", search.ListingIds.Take(10).Skip(i)), search.QueryId);
                var searchResult = GetResponseContent(response);
                if (searchResult.IsFailure) { return result.ToResult().WithErrors(searchResult.Errors); }

                result = result.Join(result);
            }

            return result;
        }
        catch (Exception e) { return e; }
    }

    public async Task<Result<string>> QueryExistingSearch(TradeSearch existingSearch)
    {
        try
        {
            var searchResult =
                await _restApi.GetExistingSearch(existingSearch.OriginalQuery.LeagueName, existingSearch.QueryId);
            var response = GetResponseStringContent(searchResult);
            if (!response.IsSuccess) return Result.Failure("Failed to get existing search by search id.");
            var html = response.Value;
            if (html.Contains("No search found")) { return Result.Failure("No search found"); }

            var json = ExtractTradeSiteJsonFromHtml(html);
            if (json == null) return Result.Failure("Failed to extract json from html");
            return ExtractStateFromJsonString(json.Value);
        }
        catch (Exception e) { return e; }
    }

    static Result<string> ExtractTradeSiteJsonFromHtml(string arg)
    {
        var regex = new Regex(@"function\(t\)\{    t\(((.|\n)*)\)\;\}\)\;\}\);");
        var match = regex.Match(arg);
        return match.Success ? match.Groups[1].Value : Result.Failure("Failed to extract json from trade site HTML.");
    }

    static Result<string> ExtractStateFromJsonString(string json)
    {
        if (json == null) return Result.Failure("Json is null");
        try
        {
            var jObject = JObject.Parse(json);
            var rawJson = jObject["state"].ToString();
            return new Regex(@"\s+").Replace(rawJson, "");
        }
        catch (Exception e) { return e; }
    }

    /// <summary>
    /// Returns an observable that will emit events specific to the given queryId.
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="queryId"></param>
    /// <returns></returns>
    public Result<LiveSearch> SubscribeLiveSearch(TradeSearch search)
    {
        try
        {
            var liveSearch = new LiveSearch(CreateWebsocketClient(search));
            _subscriptions.TryAdd(search, liveSearch);
            return liveSearch;
        }
        catch (Exception e) { return e; }
    }

    private WebsocketClient CreateWebsocketClient(TradeSearch searchQuery)
    {
        return new WebsocketClient(
            new Uri(PoeLiveSearchUrl + $"/api/trade/live/{searchQuery.LeagueName}/{searchQuery.QueryId}"),
            ClientFactory);
    }

    private ClientWebSocket ClientFactory()
    {
        var client = new ClientWebSocket();
        foreach (var header in _commonRequestHeadersProvider.Headers)
        {
            client.Options.SetRequestHeader(header.Key, header.Value);
        }

        client.Options.SetRequestHeader("Cookie", $"POESESSID={_sessionIdProvider.SessionId.Value}");
        client.Options.SetRequestHeader("Origin", "https://www.pathofexile.com");
        client.Options.SetRequestHeader("Sec-WebSocket-Extensions", "permessage-deflate; client_max_window_bits");
        return client;
    }

    public static Result<string> GetResponseStringContent<T>(Response<T> response)
    {
        try
        {
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                return new HttpResponseError(response.ResponseMessage);
            }

            return response.StringContent;
        }
        catch (Exception e) { return e; }
    }

    public static Result<T> GetResponseContent<T>(Response<T> response)
    {
        try
        {
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                return new HttpResponseError(response.ResponseMessage);
            }

            return response.GetContent();
        }
        catch (Exception e) { return e; }
    }
}

public record HttpResponseError : UnexpectedError
{
    public HttpResponseError(HttpResponseMessage message) : base(
        "The HTTP request failed with status code " + message.StatusCode + " (" + message.ReasonPhrase + ")")
    {
    }
}