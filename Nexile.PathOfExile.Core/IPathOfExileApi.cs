using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Meigs2.Functional;
using Meigs2.Functional.Enumeration;
using Meigs2.Functional.Results;
using RestEase;
using Websocket.Client;

namespace Nexile.PathOfExile;

public interface IPathOfExileApi
{
    Task<Result<ExchangePostResult>> SearchExchange(ExchangeQuery query);
    Task<Result<TradeSearch>> CreateItemSearch(ItemSearchQuery query);
    Task<Result<SearchGetResult>> GetItemSearchResults(TradeSearch search, int numberOfResults = 10);
    Task<Result<string>> QueryExistingSearch(TradeSearch existingSearch);
    
    List<LiveSearch> LiveSearches { get; }
    Result<LiveSearch> SubscribeLiveSearch(TradeSearch search);
}

public record ItemSearchQuery
{
    public string LeagueName { get; init; }
    public string QueryString { get; init; }
}

public record TradeSearch
{
    public string QueryId { get; init; }
    public decimal? Complexity { get; init; }
    public string LeagueName => OriginalQuery?.LeagueName;
    public ItemSearchQuery OriginalQuery { get; init; }
    public IReadOnlyList<string> ListingIds { get; init; }
    public int NumberOfResults { get; init; }
}

public record ExchangeSearch
{
    public string Id { get; init; }
    public decimal? Complexity { get; init; }
    public ExchangeQuery Query { get; init; }
    public IReadOnlyDictionary<string, ExchangeOffer> Offers { get; init; }
    public int NumberOfResults { get; init; }
}

public record SearchResultItemId
{
    public string ItemId { get; init; }
}

public record ExchangeQuery
{
    public string LeagueName { get; init; }
    public string QueryString { get; init; }
} 

public record LiveSearchEvent : Enumeration<LiveSearchEvent, int>
{
    public static readonly LiveSearchEvent MessageReceived = new(nameof(MessageReceived), 1);
    public static readonly LiveSearchEvent Reconnected = new(nameof(Reconnected), 2);
    public static readonly LiveSearchEvent Disconnected = new(nameof(Disconnected), 3);
    
    public Option<string> Message { get; init; }
    
    /// <inheritdoc />
    protected LiveSearchEvent(string name, int value) : base(name, value)
    {
    }
}

public record LiveSearch : IObservable<LiveSearchEvent>, IDisposable
{
    protected Subject<LiveSearchEvent> Subject { get; } = new();
    public WebsocketClient Client { get; init; }

    public LiveSearch(WebsocketClient client)
    {
        Client = client;
        client.MessageReceived.Subscribe(x => Subject.OnNext(LiveSearchEvent.MessageReceived with { Message = x.Text }));
        client.ReconnectionHappened.Subscribe(x => Subject.OnNext(LiveSearchEvent.Reconnected with { Message = x.ToString()}));
        client.DisconnectionHappened.Subscribe(x =>
        {
            Subject.OnNext(LiveSearchEvent.Disconnected with { Message = x.ToString() });
            Subject.OnCompleted();
        });
    }

    /// <inheritdoc />
    public IDisposable Subscribe(IObserver<LiveSearchEvent> observer)
    {
        return Subject.Subscribe(observer);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Subject?.Dispose();
        Client?.Dispose();
    }
}