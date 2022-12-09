using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Nexile.Common.Interfaces;
using RestEase;
using Websocket.Client;
using Meigs2.Functional;
using Meigs2.Functional.Results;
using Nexile.PathOfExile.QueryBuilder;

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

    public async Task<Result<ExchangePostResult>> QueryExchange(ExchangeQuery searchQuery)
    {
        try
        {
            var response = await _restApi.PostExchange(searchQuery.QueryString, searchQuery.LeagueName);
            return GetResponseContent(response);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<Result<ItemSearchPostResult>> QueryItemSearch(ItemSearchQuery itemSearchQuery)
    {
        try
        {
            var response = await _restApi.PostSearch(itemSearchQuery.QueryString, itemSearchQuery.LeagueName);
            return GetResponseContent(response);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<Result<SearchGetResult>> GetItemSearchResult(TradeSearch search, Func<TradeSearch, IEnumerable<ItemSearchResultPage>> pages)
    {
        try
        {
            var response = await _restApi.GetSearch(string.Join(", ", pages(search)), search.QueryId);
            return GetResponseContent(response);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<Result<string>> GetExistingSearchByQueryId(string leagueName, string queryId)
    {
        try
        {
            var searchResult = await _restApi.GetExistingSearch(leagueName, queryId);
            var response = GetResponseStringContent(searchResult);
            if (!response.IsSuccess) return Result.Failure("Failed to get existing search by query id.");
            var result = response.Value;
            if (result.Contains("No search found"))
            {
                return Result.Failure("No search found");
            }

            return ExtractStateFromJsonString(response.Value);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    static Option<string> ExtractTradeSiteJsonFromHtml(string arg)
    {
        var regex = new Regex(@"function\(t\)\{    t\(((.|\n)*)\)\;\}\)\;\}\);");
        var match = regex.Match(arg);
        return match.Success ? match.Groups[1].Value.ToSome() : Option<string>.None;
    }

    static Result<string> ExtractStateFromJsonString(string json)
    {
        if (json == null) return Result.Failure("Json is null");
        try
        {
            var jObject = JObject.Parse(json);
            return jObject["state"].ToString();
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <summary>
    /// Returns an observable that will emit events specific to the given queryId.
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="queryId"></param>
    /// <returns></returns>
    public async Task<Result<LiveSearch>> SubscribeLiveSearch(TradeSearch query)
    {
        try
        {
            var searchResult = await GetExistingSearchByQueryId(query.QueryId, query.QueryId);
            if (!searchResult.IsSuccess) return searchResult;
            var state = searchResult.Value;
            var liveSearch = new LiveSearch(query, state);
            _subscriptions.TryAdd(query, liveSearch);
            return liveSearch;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    /// <summary>
    /// Returns an observable that will emit events specific to the given queryId.
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="queryId"></param>
    /// <returns></returns>
    public async Task<bool> UnsubscribeLiveSearch(string leagueName, string queryId)
    {
        if (!_subscriptions.TryRemove((leagueName, queryId), out var client)) return await Task.FromResult(false);
        client?.Dispose();
        return await Task.FromResult(true);
    }

    private WebsocketClient CreateWebsocketClient(TradeSearchQuery searchQuery)
    {
        return new WebsocketClient(new Uri(PoeLiveSearchUrl + $"/api/trade/live/{searchQuery.LeagueName}/{searchQuery.QueryId}"),
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

    static Result<string> GetResponseStringContent<T>(Response<T> response)
    {
        try
        {
            return response.StringContent;
        }
        catch (Exception e)
        {
            return e;
        }
    }

    static Result<T> GetResponseContent<T>(Response<T> response)
    {
        try
        {
            return response.GetContent();
        }
        catch (Exception e)
        {
            return e;
        }
    }
}