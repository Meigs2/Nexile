using System;
using Newtonsoft.Json;
using NUnit.Framework;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class TestProjectExtensionMethods
    {
        public static void Log<T>(this T obj) { TestContext.WriteLine(obj.ToJsonString()); }
    }

    public static class NewtonsoftJsonExtensions
    {
        public static string ToJsonString(this object obj) { return JsonConvert.SerializeObject(obj); }

        public static T FromJsonString<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json) ?? throw new InvalidOperationException();
        }

        public static T FromJsonString<T>(this string json, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<T>(json, settings) ?? throw new InvalidOperationException();
        }
    }
}