// ReSharper disable file CheckNamespace
namespace System.Collections.Generic;

public static class EnumerableExtensions
{
    public static IEnumerable<IEnumerable<T>> Partition<T>(this IEnumerable<T> values, int chunkSize)
    {
        while (values.Any())
        {
            yield return values.Take(chunkSize).ToList();
            values = values.Skip(chunkSize).ToList();
        }
    }
}