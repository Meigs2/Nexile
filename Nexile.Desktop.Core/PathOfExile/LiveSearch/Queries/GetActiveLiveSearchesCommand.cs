using MediatR;
using Meigs2.Functional;
using Nexile.PathOfExile;

namespace Nexile.Desktop.Core.PathOfExile.LiveSearch.Queries;

public record ActiveLiveSearchResult(string LeagueName, string QueryId);
public record GetActiveLiveSearchesCommand : IRequest<IEnumerable<ActiveLiveSearchResult>>;

public class GetActiveLiveSearchesCommandHandler : IRequestHandler<GetActiveLiveSearchesCommand, IEnumerable<ActiveLiveSearchResult>>
{
    private readonly IPathOfExileApi _api;

    public GetActiveLiveSearchesCommandHandler(IPathOfExileApi api)
    {
        _api = api;
    }

    /// <inheritdoc />
    public Task<IEnumerable<ActiveLiveSearchResult>> Handle(GetActiveLiveSearchesCommand request, CancellationToken cancellationToken)
    {
        return _api.LiveSearches.Map(x => new ActiveLiveSearchResult(x.Item1, x.Item2)).ToTask();
    }
}
