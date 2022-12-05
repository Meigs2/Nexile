using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Meigs2.Functional;
using Meigs2.Functional.Results;

namespace Nexile.PathOfExile;

public interface IPathOfExileApi
{
    Task<Result<ExchangePostResult>> PostExchange(string query, string leagueName);
    Task<Result<SearchPostResult>> PostSearch(string query, string leagueName);
    Task<Result<SearchGetResult>> GetSearch(string resultIds, string queryId);
    Task<Result<string>> GetExistingSearchByQueryId(string leagueName, string queryId);

    /// <inheritdoc />
    Task<Result<IObservable<LiveSearchEvent>>> SubscribeLiveSearch(string leagueName, string queryId);

    public record LiveSearchEvent(LiveSearchEventType Type, string LeagueName, string QueryId, string Message);

    public enum LiveSearchEventType
    {
        Received,
        Reconnected,
        Connected,
        Disconnected
    }

    List<(string, string)> ActiveSearches { get; }
}