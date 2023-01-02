using Meigs2.Functional;

namespace Nexile.PathOfExile;

public class SessionIdProvider : ISessionIdProvider
{
    public SessionIdProvider(SessionIdOptions options)
    {
        SessionId = options.SessionId;
    }
    
    /// <inheritdoc />
    public Option<string> SessionId { get; internal set; }
    
    public void SetSessionId(string sessionId) => SessionId = sessionId;
    public void ClearSessionId() => SessionId = Option.None;
}