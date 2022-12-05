using Newtonsoft.Json;

namespace Nexile.Desktop.Core.Common.Extensions;

public static class NewtonsoftJsonExtensions
{
    public static string ToJsonString(this object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }
    
    public static T FromJsonString<T>(this string json)
    {
        return JsonConvert.DeserializeObject<T>(json) ?? throw new InvalidOperationException();
    }
    
    public static T FromJsonString<T>(this string json, JsonSerializerSettings settings)
    {
        return JsonConvert.DeserializeObject<T>(json, settings) ?? throw new InvalidOperationException();
    }
}