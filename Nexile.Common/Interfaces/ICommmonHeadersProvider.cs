using System.Collections.Generic;

namespace Nexile.Common.Interfaces;

public interface ICommmonHeadersProvider
{
    List<KeyValuePair<string,string>> Headers { get; }
}