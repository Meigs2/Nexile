using Meigs2.Functional;

namespace Nexile.PathOfExile;

public interface IPoeSessionIdProvider
{
    public Option<string> SessionId { get; }
    void SetSessionId(string sessionId);
    void ClearSessionId();
}