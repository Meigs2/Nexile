using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestEase;

namespace Nexile.PathOfExile;

public interface IOfficialTradeRestApi
{
    [Post("/api/trade/exchange/{leagueName}")]
    [Header("Content-Type", "application/json")]
    Task<Response<ExchangePostResult>> PostExchange([Body] string query, [Path] string leagueName);

    [Post("/api/trade/search/{leagueName}")]
    [Header("Content-Type", "application/json")]
    Task<Response<SearchPostResult>> PostSearch([Body] string query, [Path] string leagueName);

    [Get("/api/trade/fetch/{ids}")]
    Task<Response<SearchGetResult>> GetSearch([Path("ids")] string resultIds, [Query("query")] string queryId);

    [Get("/trade/search/{leagueName}/{queryId}")]
    Task<Response<string>> GetExistingSearch([Path] string leagueName, [Path] string queryId);
}

#region Exchange Results

public record ExchangeOffer([property: JsonProperty("id")] string Id, [property: JsonProperty("item")] JObject Item,
    [property: JsonProperty("listing")] ExchangeDetails Listing);

public record Online([property: JsonProperty("league")] string League,
    [property: JsonProperty("status")] string Status);

public record Account([property: JsonProperty("name")] string Name, [property: JsonProperty("lastCharacterName")]
    string LastCharacterName, [property: JsonProperty("online")] Online Online,
    [property: JsonProperty("language")] string Language);

public record Exchange([property: JsonProperty("currency")] string Currency,
    [property: JsonProperty("amount")] decimal Amount, [property: JsonProperty("whisper")] string Whisper);

public record ExchangeItem([property: JsonProperty("currency")] string Currency,
    [property: JsonProperty("amount")] decimal Amount, [property: JsonProperty("stock")] int Stock,
    [property: JsonProperty("id")] string Id, [property: JsonProperty("whisper")] string Whisper);

public record ExchangeDetails([property: JsonProperty("indexed")] DateTime Indexed,
    [property: JsonProperty("account")] Account Account, [property: JsonProperty("offers")] IReadOnlyList<Offer> Offers,
    [property: JsonProperty("whisper")] string Whisper);

public record Offer([property: JsonProperty("exchange")] Exchange Exchange,
    [property: JsonProperty("item")] ExchangeItem Item);

public record ExchangePostResult([property: JsonProperty("id")] string Id,
    [property: JsonProperty("complexity")] double? Complexity,
    [property: JsonProperty("result")] IReadOnlyDictionary<string, ExchangeOffer> Offers,
    [property: JsonProperty("total")] int Total);

#endregion

#region Search Results

public record SearchPostResult([property: JsonProperty("id")] string QueryId,
    [property: JsonProperty("complexity")] int Complexity,
    [property: JsonProperty("result")] IReadOnlyList<string> Results, [property: JsonProperty("total")] int Total);

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

public record Crafted([property: JsonProperty("name")] string Name, [property: JsonProperty("tier")] string Tier,
    [property: JsonProperty("level")] int Level,
    [property: JsonProperty("magnitudes")] IReadOnlyList<Magnitude> Magnitudes);

public record Explicit([property: JsonProperty("name")] string Name, [property: JsonProperty("tier")] string Tier,
    [property: JsonProperty("level")] int Level,
    [property: JsonProperty("magnitudes")] IReadOnlyList<Magnitude> Magnitudes);

public record Extended([property: JsonProperty("dps")] double Dps, [property: JsonProperty("pdps")] double Pdps,
    [property: JsonProperty("edps")] double Edps, [property: JsonProperty("dps_aug")] bool DpsAug,
    [property: JsonProperty("pdps_aug")] bool PdpsAug, [property: JsonProperty("mods")] Mods Mods,
    [property: JsonProperty("hashes")] Hashes Hashes, [property: JsonProperty("text")] string Text);

public record Hashes([property: JsonProperty("explicit")] IReadOnlyList<List<JObject>> Explicit,
    [property: JsonProperty("implicit")] IReadOnlyList<List<JObject>> Implicit,
    [property: JsonProperty("crafted")] IReadOnlyList<List<JObject>> Crafted);

public record Implicit([property: JsonProperty("name")] string Name, [property: JsonProperty("tier")] string Tier,
    [property: JsonProperty("level")] int Level,
    [property: JsonProperty("magnitudes")] IReadOnlyList<Magnitude> Magnitudes);

public record Influences([property: JsonProperty("shaper")] bool Shaper);

public record Item([property: JsonProperty("verified")] bool Verified, [property: JsonProperty("w")] int W,
    [property: JsonProperty("h")] int H, [property: JsonProperty("icon")] string Icon,
    [property: JsonProperty("league")] string League, [property: JsonProperty("id")] string Id,
    [property: JsonProperty("sockets")] IReadOnlyList<Socket> Sockets, [property: JsonProperty("name")] string Name,
    [property: JsonProperty("typeLine")] string TypeLine, [property: JsonProperty("baseType")] string BaseType,
    [property: JsonProperty("identified")] bool Identified, [property: JsonProperty("ilvl")] int Ilvl,
    [property: JsonProperty("note")] string Note,
    [property: JsonProperty("properties")] IReadOnlyList<Property> Properties, [property: JsonProperty("requirements")]
    IReadOnlyList<Requirement> Requirements, [property: JsonProperty("implicitMods")]
    IReadOnlyList<string> ImplicitMods, [property: JsonProperty("explicitMods")]
    IReadOnlyList<string> ExplicitMods, [property: JsonProperty("frameType")] int FrameType,
    [property: JsonProperty("extended")] Extended Extended, [property: JsonProperty("craftedMods")]
    IReadOnlyList<string> CraftedMods, [property: JsonProperty("flavourText")]
    IReadOnlyList<string> FlavourText, [property: JsonProperty("corrupted")] bool? Corrupted,
    [property: JsonProperty("influences")] Influences Influences, [property: JsonProperty("shaper")] bool? Shaper);

public record Listing([property: JsonProperty("method")] string Method,
    [property: JsonProperty("indexed")] DateTime Indexed, [property: JsonProperty("stash")] Stash Stash,
    [property: JsonProperty("whisper")] string Whisper, [property: JsonProperty("account")] Account Account,
    [property: JsonProperty("price")] Price Price);

public record Magnitude([property: JsonProperty("hash")] string Hash, [property: JsonProperty("min")] double Min,
    [property: JsonProperty("max")] double Max);

public record Mods([property: JsonProperty("explicit")] IReadOnlyList<Explicit> Explicit,
    [property: JsonProperty("implicit")] IReadOnlyList<Implicit> Implicit,
    [property: JsonProperty("crafted")] IReadOnlyList<Crafted> Crafted);

public record Price([property: JsonProperty("tag")] string Tag, [property: JsonProperty("type")] string Type,
    [property: JsonProperty("amount")] double Amount, [property: JsonProperty("currency")] string Currency);

public record Property([property: JsonProperty("name")] string Name,
    [property: JsonProperty("values")] IReadOnlyList<List<JObject>> Values, [property: JsonProperty("displayMode")]
    int DisplayMode, [property: JsonProperty("type")] int? Type);

public record Requirement([property: JsonProperty("name")] string Name,
    [property: JsonProperty("values")] IReadOnlyList<List<JObject>> Values, [property: JsonProperty("displayMode")]
    int DisplayMode, [property: JsonProperty("type")] int Type);

public record SearchResult([property: JsonProperty("id")] string Id,
    [property: JsonProperty("listing")] Listing Listing, [property: JsonProperty("item")] Item Item);

public record SearchGetResult([property: JsonProperty("result")] IReadOnlyList<SearchResult> Results);

public record Socket([property: JsonProperty("group")] int Group, [property: JsonProperty("attr")] string Attr,
    [property: JsonProperty("sColour")] string SColour);

public record Stash([property: JsonProperty("name")] string Name, [property: JsonProperty("x")] int X,
    [property: JsonProperty("y")] int Y);

#endregion