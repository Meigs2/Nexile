using MediatR;

namespace Nexile.Desktop.Domain.PathOfExile.LiveSearch.Events;

public record LiveSearchResultReceived(string LeagueName, string QueryId, string Response) : INotification;