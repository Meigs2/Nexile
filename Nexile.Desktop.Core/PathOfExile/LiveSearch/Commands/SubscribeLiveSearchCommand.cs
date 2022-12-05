using MediatR;
using Meigs2.Functional;
using Meigs2.Functional.Results;
using Nexile.PathOfExile;
using static Nexile.PathOfExile.IPathOfExileApi;

namespace Nexile.Desktop.Core.PathOfExile.LiveSearch.Commands;

public record SubscribeLiveSearchCommand
    (string LeagueName, string QueryId) : IRequest<Result<IObservable<LiveSearchEvent>>>;

public class
    SubscribeLiveSearchCommandHandler : IRequestHandler<SubscribeLiveSearchCommand,
        Result<IObservable<LiveSearchEvent>>>
{
    private IPathOfExileApi _api;

    public SubscribeLiveSearchCommandHandler(IPathOfExileApi api)
    {
        _api = api;
    }

    /// <inheritdoc />
    public Task<Result<IObservable<LiveSearchEvent>>> Handle(SubscribeLiveSearchCommand request,
                                                             CancellationToken cancellationToken)
    {
        return _api.SubscribeLiveSearch(request.LeagueName, request.QueryId);
    }
}