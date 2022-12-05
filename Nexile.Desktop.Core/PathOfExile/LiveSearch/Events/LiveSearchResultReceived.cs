using MediatR;

namespace Nexile.Desktop.Core.PathOfExile.LiveSearch.Events;

public record LiveSearchResultReceived(string LeagueName, string QueryId, string Response) : INotification;