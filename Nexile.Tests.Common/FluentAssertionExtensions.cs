using System;
using FluentAssertions.Numeric;

namespace Nexile.Tests.Common;

public static class FluentAssertionExtensions
{
    public static ComparableTypeAssertions<T> Should<T>(this IComparable<T> comparableValue)
    {
        return new ComparableTypeAssertions<T>(comparableValue);
    }
}