using System;
using System.Runtime.CompilerServices;
using Meigs2.Functional;
using NUnit.Framework;

// ReSharper disable once CheckNamespace
namespace System;

public static class LanguageExtExtensions
{
    public static T ThrowIfNone<T>(this Option<T> result, [CallerMemberName] string methodName = "")
    {
        if (result.IsNone)
        {
            throw new Exception($"Method {methodName} returned None");
        }

        return result.Value;
    }
}