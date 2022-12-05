using System.Collections.Generic;
using Nexile.Common.Interfaces;

namespace Nexile.Common;

public class NexileUserAgentHeaderProvider : ICommonRequestHeadersProvider
{
    /// <inheritdoc />
    public List<KeyValuePair<string, string>> Headers { get; } = new()
                                                                 {
                                                                     new KeyValuePair<string, string>(
                                                                         "user-agent",
                                                                         "Nexile PoE Tool/0.0.1 Windows (PoE Account: Meigs2, Contact: ConnorRadeloff1@gmail.com)"),
                                                                 };
}