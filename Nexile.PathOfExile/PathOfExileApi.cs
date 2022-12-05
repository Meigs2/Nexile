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

namespace Nexile.PathOfExile;

public class PathOfExileApi : IPathOfExileApi
{
    private readonly IOfficialTradeRestApi _restApi;
    private readonly IPoeSessionIdService _sessionIdService;
    private readonly ICommonRequestHeadersProvider _commonRequestHeadersProvider;
    public const string PoeLiveSearchUrl = "wss://www.pathofexile.com";
    public const string PoeWebsiteUrl = "https://www.pathofexile.com";
    private Subject<IPathOfExileApi.LiveSearchEvent> LiveSearchSubject { get; } = new();
    private ConcurrentDictionary<(string, string), WebsocketClient> _clients = new();
    public List<(string, string)> ActiveSearches => _clients.Keys.ToList();

    /// <summary>
    /// Entire feed of live search events.
    /// </summary>
    public IObservable<IPathOfExileApi.LiveSearchEvent> LiveSearchListingNotifications => LiveSearchSubject
        .AsObservable()
        .Where(e => e.Type == IPathOfExileApi.LiveSearchEventType.Received);

    public IObservable<IPathOfExileApi.LiveSearchEvent> LiveSearchEvents => LiveSearchSubject.AsObservable();

    public PathOfExileApi(IOfficialTradeRestApi restApi,
                          IPoeSessionIdService sessionIdService,
                          ICommonRequestHeadersProvider commonRequestHeadersProvider)
    {
        _restApi = restApi;
        _sessionIdService = sessionIdService;
        _commonRequestHeadersProvider = commonRequestHeadersProvider;
    }

    public async Task<Result<ExchangePostResult>> PostExchange(string query, string leagueName)
    {
        try
        {
            var response = await _restApi.PostExchange(query, leagueName);
            return GetResponseContent(response);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<Result<SearchPostResult>> PostSearch(string query, string leagueName)
    {
        try
        {
            var response = await _restApi.PostSearch(query, leagueName);
            return GetResponseContent(response);
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public async Task<Result<SearchGetResult>> GetSearch(string resultIds, string queryId)
    {
        try
        {
            var response = await _restApi.GetSearch(resultIds, queryId);
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
    public async Task<Result<IObservable<IPathOfExileApi.LiveSearchEvent>>> SubscribeLiveSearch(
        string leagueName,
        string queryId)
    {
        var client = CreateWebsocketClient(leagueName, queryId);
        if (_clients.ContainsKey((leagueName, queryId)))
        {
            // if we do, check if it's connected
            if (_clients[(leagueName, queryId)].IsRunning)
            {
                // if it is, return the existing observable
                return LiveSearchSubject.AsObservable()
                                        .Where(e => e.LeagueName == leagueName && e.QueryId == queryId).ToResult();
            }

            // if it's not connected, remove it from the dictionary
            _clients.TryRemove((leagueName, queryId), out var existingClient);
            existingClient?.Dispose();
        }

        client.MessageReceived.Subscribe(msg =>
        {
            var liveSearchEvent
                = new IPathOfExileApi.LiveSearchEvent(IPathOfExileApi.LiveSearchEventType.Received, leagueName, queryId,
                                                      msg.Text);
            LiveSearchSubject.OnNext(liveSearchEvent);
        });
        client.DisconnectionHappened.Subscribe(msg =>
        {
            if (msg.Type == DisconnectionType.Exit)
            {
                return;
            }

            var liveSearchEvent = new IPathOfExileApi.LiveSearchEvent(IPathOfExileApi.LiveSearchEventType.Disconnected,
                                                                      leagueName, queryId, msg.CloseStatusDescription);
            LiveSearchSubject.OnNext(liveSearchEvent);
            client?.Dispose();
        });
        client.ReconnectionHappened.Subscribe(msg =>
        {
            var liveSearchEvent = new IPathOfExileApi.LiveSearchEvent(IPathOfExileApi.LiveSearchEventType.Reconnected,
                                                                      leagueName, queryId, msg.Type.ToString());
            if (msg.Type == ReconnectionType.Initial)
            {
                liveSearchEvent = new IPathOfExileApi.LiveSearchEvent(IPathOfExileApi.LiveSearchEventType.Connected,
                                                                      leagueName, queryId, msg.Type.ToString());
            }

            LiveSearchSubject.OnNext(liveSearchEvent);
        });
        try
        {
            await client.StartOrFail();
        }
        catch (Exception e)
        {
            return e;
        }

        _clients.TryAdd((leagueName, queryId), client);
        return LiveSearchSubject.Where(e => e.LeagueName == leagueName && e.QueryId == queryId).ToResult();
    }

    /// <summary>
    /// Returns an observable that will emit events specific to the given queryId.
    /// </summary>
    /// <param name="leagueName"></param>
    /// <param name="queryId"></param>
    /// <returns></returns>
    public async Task<bool> UnsubscribeLiveSearch(string leagueName, string queryId)
    {
        if (!_clients.TryRemove((leagueName, queryId), out var client)) return await Task.FromResult(false);
        client?.Dispose();
        return await Task.FromResult(true);
    }

    private WebsocketClient CreateWebsocketClient(string leagueName, string queryId)
    {
        return new WebsocketClient(new Uri(PoeLiveSearchUrl + $"/api/trade/live/{leagueName}/{queryId}"),
                                   ClientFactory);
    }

    private ClientWebSocket ClientFactory()
    {
        var client = new ClientWebSocket();
        foreach (var header in _commonRequestHeadersProvider.Headers)
        {
            client.Options.SetRequestHeader(header.Key, header.Value);
        }

        client.Options.SetRequestHeader("Cookie", $"POESESSID={_sessionIdService.SessionId.Value}");
        client.Options.SetRequestHeader("Origin", "https://www.pathofexile.com");
        client.Options.SetRequestHeader("Sec-WebSocket-Extensions", "permessage-deflate; client_max_window_bits");
        return client;
    }

    static Result<string> GetResponseStringContent<T>(Response<T> response)
    {
        return response.StringContent;
    }

    static Result<T> GetResponseContent<T>(Response<T> response)
    {
        return response.GetContent();
    }
}