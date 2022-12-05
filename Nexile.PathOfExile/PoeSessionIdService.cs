using Meigs2.Functional;

namespace Nexile.PathOfExile;

public class PoeSessionIdService : IPoeSessionIdService
{
    public PoeSessionIdService(SessionIdOptions options)
    {
        SessionId = options.SessionId;
    }
    
    /// <inheritdoc />
    public Option<string> SessionId { get; internal set; }
    
    // expose a method to set the session id
    public void SetSessionId(string sessionId) => SessionId = sessionId;
    // clear
    public void ClearSessionId() => SessionId = Option<string>.None;
}