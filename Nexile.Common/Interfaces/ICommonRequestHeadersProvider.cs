using System.Collections.Generic;

namespace Nexile.Common.Interfaces;

public interface ICommonRequestHeadersProvider
{
    List<KeyValuePair<string,string>> Headers { get; }
}