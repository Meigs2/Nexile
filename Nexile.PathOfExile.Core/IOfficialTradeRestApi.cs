using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Meigs2.Functional;
using Meigs2.Functional.Common;
using Meigs2.Functional.Enumeration;
using Meigs2.Functional.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestEase;
using static Meigs2.Functional.F;
using F = Meigs2.Functional.F;

namespace Nexile.PathOfExile;

public interface IOfficialTradeRestApi
{
    [Post("/api/trade/exchange/{leagueName}")]
    [Header("Content-Type", "application/json")]
    Task<Response<ExchangePostResult>> PostExchange([Body] string query, [Path] string leagueName);

    [Post("/api/trade/search/{leagueName}")]
    [Header("Content-Type", "application/json")]
    Task<Response<ItemSearch>> PostSearch([Body] string query, [Path] string leagueName);

    [Get("/api/trade/fetch/{ids}")]
    Task<Response<SearchGetResult>> GetSearch([Path("ids")] string resultIds, [Query("query")] string queryId);

    [Get("/trade/search/{leagueName}/{queryId}")]
    Task<Response<string>> GetExistingSearch([Path] string leagueName, [Path] string queryId);

    [Get("/trade/data/items")]
    Task<Response<ItemCategoryResult>> GetItemData();

    [Get("/api/trade/data/stats")]
    Task<Response<ItemStatResult>> GetItemStats();

    [Get("/api/trade/data/static")]
    Task<Response<StaticDataResult>> GetStaticData();
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Entry
{
    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("flags")]
    public ItemFlag ItemFlag { get; set; }

    [JsonProperty("disc")]
    public string Disc { get; set; }
}

public class ItemFlag
{
    [JsonProperty("unique")]
    public bool Unique { get; set; }
}

public class ItemCategory
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("label")]
    public string Label { get; set; }

    [JsonProperty("entries")]
    public List<Entry> Entries { get; set; }
}

public class ItemCategoryResult
{
    [JsonProperty("result")]
    public List<ItemCategory> Result { get; set; }
}


// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class StatLine
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("option")]
    public OptionsObject OptionsObject { get; set; }
}

public class OptionsObject
{
    [JsonProperty("options")]
    public List<StatOption> Options { get; set; }
}

public class StatOption
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }
}

public class ItemStat
{
    [JsonProperty("label")]
    public string Label { get; set; }

    [JsonProperty("entries")]
    public List<Entry> Entries { get; set; }
}

public class ItemStatResult
{
    [JsonProperty("result")]
    public List<ItemStat> Result { get; set; }
}


// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class StaticData
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("image")]
    public string Image { get; set; }
}

public class StaticDataCategory
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("label")]
    public string Label { get; set; }

    [JsonProperty("entries")]
    public List<StaticData> Entries { get; set; }
}

public class StaticDataResult
{
    [JsonProperty("result")]
    public List<StaticDataCategory> Result { get; set; }
}

public interface IPoeOauthApi
{
    [Get("/league")]
    // Add the optional parameters for realm, type (main, event, season), limit (Max 50) and offset (use with limit)
    // For the enum and limit, create classes to represent them
    Task<Response<List<League>>> GetLeagues([Query("realm")] Realm realm,
        [Query("type")] LeagueType type,
        [Query("limit")] int limit,
        [Query("offset")] int offset);
}

public record RetryPolicy
{
    public string Policy { get; init; }
    public string[] Rules { get; init; }
    public Dictionary<string, Limit> Limits { get; init; }
    public Dictionary<string, State> States { get; init; }
    public int RetryAfter { get; init; }

    public static RetryPolicy FromHttpResponse(HttpResponseMessage response)
    {
        var policy = "";
        if (response.Headers.TryGetValues("X-Rate-Limit-Policy", out var policyValues))
        {
            policy = policyValues.FirstOrDefault();
        }

        List<string> rules = new();
        if (response.Headers.TryGetValues("X-Rate-Limit-Rules", out var ruleValues))
        {
            rules = ruleValues.FirstOrDefault().Split(',').ToList();
        }

        var limits = new Dictionary<string, Limit>();
        var states = new Dictionary<string, State>();
        foreach (var rule in rules)
        {
            if (response.Headers.TryGetValues($"X-Rate-Limit-{rule}", out var limitValues))
            {
                var parts = limitValues.FirstOrDefault().Split(':');
                limits[rule] = new Limit(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
            }

            if (response.Headers.TryGetValues($"X-Rate-Limit-{rule}-State", out var stateValues))
            {
                var parts = stateValues.FirstOrDefault().Split(':');
                states[rule] = new State(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
            }
        }

        var retryAfter = 0;
        if (response.Headers.TryGetValues("Retry-After", out var retryAfterValues))
        {
            retryAfter = int.Parse(retryAfterValues.FirstOrDefault());
        }

        return new RetryPolicy
        {
            Policy = policy,
            Rules = rules.ToArray(),
            Limits = limits,
            States = states,
            RetryAfter = retryAfter
        };
    }
}

public record Limit(int MaximumHits, int Period, int TimeRestricted);

public record State(int CurrentHitCount, int Period, int ActiveTimeRestricted);

public record LeagueType : Enumeration<LeagueType, string>
{
    public static LeagueType Main = new("Main", "main");
    public static LeagueType Event = new("Event", "event");
    public static LeagueType Season = new("Season", "season");
    private LeagueType(string name, string value) : base(name, value) { }
    public override string ToString() { return Value; }
}

public record Realm : Enumeration<Realm, string>
{
    public static Realm Pc = new("PC", "pc");
    public static Realm Xbox = new("Xbox", "xbox");
    public static Realm Ps4 = new("Sony", "sony");
    private Realm(string name, string value) : base(name, value) { }
    public override string ToString() { return Value; }
}

public record League
{
    public string Id { get; init; }
    public string Realm { get; init; }
    public string Description { get; init; }
    public string Rules { get; init; }
    public string RegisterAt { get; init; }
    public bool Event { get; init; }
    public string Url { get; init; }
    public string StartAt { get; init; }
    public string EndAt { get; init; }
    public bool TimedEvent { get; init; }
    public bool ScoreEvent { get; init; }
    public bool DelveEvent { get; init; }
}

#region Exchange ListingIds

public record ExchangeOffer
{
    public ExchangeOffer(string Id, JObject Item, ExchangeDetails Listing)
    {
        this.Id = Id;
        this.Item = Item;
        this.Listing = Listing;
    }

    [JsonProperty("id")]
    public string Id { get; init; }

    [JsonProperty("item")]
    public JObject Item { get; init; }

    [JsonProperty("listing")]
    public ExchangeDetails Listing { get; init; }
}

public record Online
{
    public Online(string League, string Status)
    {
        this.League = League;
        this.Status = Status;
    }

    [JsonProperty("league")]
    public string League { get; init; }

    [JsonProperty("status")]
    public string Status { get; init; }
}

public record Account
{
    public Account(string Name, string LastCharacterName, Online Online, string Language)
    {
        this.Name = Name;
        this.LastCharacterName = LastCharacterName;
        this.Online = Online;
        this.Language = Language;
    }

    [JsonProperty("name")]
    public string Name { get; init; }

    [JsonProperty("lastCharacterName")]
    public string LastCharacterName { get; init; }

    [JsonProperty("online")]
    public Online Online { get; init; }

    [JsonProperty("language")]
    public string Language { get; init; }
}

public record Exchange
{
    public Exchange(string Currency, decimal Amount, string Whisper)
    {
        this.Currency = Currency;
        this.Amount = Amount;
        this.Whisper = Whisper;
    }

    [JsonProperty("currency")]
    public string Currency { get; init; }

    [JsonProperty("amount")]
    public decimal Amount { get; init; }

    [JsonProperty("whisper")]
    public string Whisper { get; init; }
}

public record ExchangeItem
{
    public ExchangeItem(string Currency, decimal Amount, int Stock, string Id, string Whisper)
    {
        this.Currency = Currency;
        this.Amount = Amount;
        this.Stock = Stock;
        this.Id = Id;
        this.Whisper = Whisper;
    }

    [JsonProperty("currency")]
    public string Currency { get; init; }

    [JsonProperty("amount")]
    public decimal Amount { get; init; }

    [JsonProperty("stock")]
    public int Stock { get; init; }

    [JsonProperty("id")]
    public string Id { get; init; }

    [JsonProperty("whisper")]
    public string Whisper { get; init; }
}

public record ExchangeDetails
{
    public ExchangeDetails(DateTime Indexed, Account Account, IReadOnlyList<Offer> Offers, string Whisper)
    {
        this.Indexed = Indexed;
        this.Account = Account;
        this.Offers = Offers;
        this.Whisper = Whisper;
    }

    [JsonProperty("indexed")]
    public DateTime Indexed { get; init; }

    [JsonProperty("account")]
    public Account Account { get; init; }

    [JsonProperty("offers")]
    public IReadOnlyList<Offer> Offers { get; init; }

    [JsonProperty("whisper")]
    public string Whisper { get; init; }
}

public record Offer
{
    public Offer(Exchange Exchange, ExchangeItem Item)
    {
        this.Exchange = Exchange;
        this.Item = Item;
    }

    [JsonProperty("exchange")]
    public Exchange Exchange { get; init; }

    [JsonProperty("item")]
    public ExchangeItem Item { get; init; }
}

public record ExchangePostResult
{
    public ExchangePostResult(string Id,
        decimal? Complexity,
        IReadOnlyDictionary<string, ExchangeOffer> Offers,
        int Total)
    {
        this.Id = Id;
        this.Complexity = Complexity;
        this.Offers = Offers;
        this.Total = Total;
    }

    [JsonProperty("id")]
    public string Id { get; init; }

    [JsonProperty("complexity")]
    public decimal? Complexity { get; init; }

    [JsonProperty("result")]
    public IReadOnlyDictionary<string, ExchangeOffer> Offers { get; init; }

    [JsonProperty("total")]
    public int Total { get; init; }
}

#endregion

#region Search ListingIds

public record ItemSearch
{
    public ItemSearch(string QueryId, decimal Complexity, IReadOnlyList<string> Results, int Total)
    {
        this.QueryId = QueryId;
        this.Complexity = Complexity;
        this.Results = Results;
        this.Total = Total;
    }

    [JsonProperty("id")]
    public string QueryId { get; init; }

    [JsonProperty("complexity")]
    public decimal Complexity { get; init; }

    [JsonProperty("result")]
    public IReadOnlyList<string> Results { get; init; }

    [JsonProperty("total")]
    public int Total { get; init; }
}

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

public record Crafted
{
    public Crafted(string Name, string Tier, int Level, IReadOnlyList<Magnitude> Magnitudes)
    {
        this.Name = Name;
        this.Tier = Tier;
        this.Level = Level;
        this.Magnitudes = Magnitudes;
    }

    [JsonProperty("name")]
    public string Name { get; init; }

    [JsonProperty("tier")]
    public string Tier { get; init; }

    [JsonProperty("level")]
    public int Level { get; init; }

    [JsonProperty("magnitudes")]
    public IReadOnlyList<Magnitude> Magnitudes { get; init; }
}

public record Explicit
{
    public Explicit(string Name, string Tier, int Level, IReadOnlyList<Magnitude> Magnitudes)
    {
        this.Name = Name;
        this.Tier = Tier;
        this.Level = Level;
        this.Magnitudes = Magnitudes;
    }

    [JsonProperty("name")]
    public string Name { get; init; }

    [JsonProperty("tier")]
    public string Tier { get; init; }

    [JsonProperty("level")]
    public int Level { get; init; }

    [JsonProperty("magnitudes")]
    public IReadOnlyList<Magnitude> Magnitudes { get; init; }
}

public record Extended
{
    public Extended(double Dps,
        double Pdps,
        double Edps,
        bool DpsAug,
        bool PdpsAug,
        Mods Mods,
        Hashes Hashes,
        string Text)
    {
        this.Dps = Dps;
        this.Pdps = Pdps;
        this.Edps = Edps;
        this.DpsAug = DpsAug;
        this.PdpsAug = PdpsAug;
        this.Mods = Mods;
        this.Hashes = Hashes;
        this.Text = Text;
    }

    [JsonProperty("dps")]
    public double Dps { get; init; }

    [JsonProperty("pdps")]
    public double Pdps { get; init; }

    [JsonProperty("edps")]
    public double Edps { get; init; }

    [JsonProperty("dps_aug")]
    public bool DpsAug { get; init; }

    [JsonProperty("pdps_aug")]
    public bool PdpsAug { get; init; }

    [JsonProperty("mods")]
    public Mods Mods { get; init; }

    [JsonProperty("hashes")]
    public Hashes Hashes { get; init; }

    [JsonProperty("text")]
    public string Text { get; init; }
}

public record Hashes
{
    public Hashes(IReadOnlyList<List<JObject>> Explicit,
        IReadOnlyList<List<JObject>> Implicit,
        IReadOnlyList<List<JObject>> Crafted)
    {
        this.Explicit = Explicit;
        this.Implicit = Implicit;
        this.Crafted = Crafted;
    }

    [JsonProperty("explicit")]
    public IReadOnlyList<List<JObject>> Explicit { get; init; }

    [JsonProperty("implicit")]
    public IReadOnlyList<List<JObject>> Implicit { get; init; }

    [JsonProperty("crafted")]
    public IReadOnlyList<List<JObject>> Crafted { get; init; }
}

public record Implicit
{
    public Implicit(string Name, string Tier, int Level, IReadOnlyList<Magnitude> Magnitudes)
    {
        this.Name = Name;
        this.Tier = Tier;
        this.Level = Level;
        this.Magnitudes = Magnitudes;
    }

    [JsonProperty("name")]
    public string Name { get; init; }

    [JsonProperty("tier")]
    public string Tier { get; init; }

    [JsonProperty("level")]
    public int Level { get; init; }

    [JsonProperty("magnitudes")]
    public IReadOnlyList<Magnitude> Magnitudes { get; init; }
}

public record Influences
{
    public Influences(bool Shaper) { this.Shaper = Shaper; }

    [JsonProperty("shaper")]
    public bool Shaper { get; init; }
}

public record Item
{
    public Item(bool Verified,
        int W,
        int H,
        string Icon,
        string League,
        string Id,
        IReadOnlyList<Socket> Sockets,
        string Name,
        string TypeLine,
        string BaseType,
        bool Identified,
        int Ilvl,
        string Note,
        IReadOnlyList<Property> Properties,
        IReadOnlyList<Requirement> Requirements,
        IReadOnlyList<string> ImplicitMods,
        IReadOnlyList<string> ExplicitMods,
        int FrameType,
        Extended Extended,
        IReadOnlyList<string> CraftedMods,
        IReadOnlyList<string> FlavourText,
        bool? Corrupted,
        Influences Influences,
        bool? Shaper)
    {
        this.Verified = Verified;
        this.W = W;
        this.H = H;
        this.Icon = Icon;
        this.League = League;
        this.Id = Id;
        this.Sockets = Sockets;
        this.Name = Name;
        this.TypeLine = TypeLine;
        this.BaseType = BaseType;
        this.Identified = Identified;
        this.Ilvl = Ilvl;
        this.Note = Note;
        this.Properties = Properties;
        this.Requirements = Requirements;
        this.ImplicitMods = ImplicitMods;
        this.ExplicitMods = ExplicitMods;
        this.FrameType = FrameType;
        this.Extended = Extended;
        this.CraftedMods = CraftedMods;
        this.FlavourText = FlavourText;
        this.Corrupted = Corrupted;
        this.Influences = Influences;
        this.Shaper = Shaper;
    }

    [JsonProperty("verified")]
    public bool Verified { get; init; }

    [JsonProperty("w")]
    public int W { get; init; }

    [JsonProperty("h")]
    public int H { get; init; }

    [JsonProperty("icon")]
    public string Icon { get; init; }

    [JsonProperty("league")]
    public string League { get; init; }

    [JsonProperty("id")]
    public string Id { get; init; }

    [JsonProperty("sockets")]
    public IReadOnlyList<Socket> Sockets { get; init; }

    [JsonProperty("name")]
    public string Name { get; init; }

    [JsonProperty("typeLine")]
    public string TypeLine { get; init; }

    [JsonProperty("baseType")]
    public string BaseType { get; init; }

    [JsonProperty("identified")]
    public bool Identified { get; init; }

    [JsonProperty("ilvl")]
    public int Ilvl { get; init; }

    [JsonProperty("note")]
    public string Note { get; init; }

    [JsonProperty("properties")]
    public IReadOnlyList<Property> Properties { get; init; }

    [JsonProperty("requirements")]
    public IReadOnlyList<Requirement> Requirements { get; init; }

    [JsonProperty("implicitMods")]
    public IReadOnlyList<string> ImplicitMods { get; init; }

    [JsonProperty("explicitMods")]
    public IReadOnlyList<string> ExplicitMods { get; init; }

    [JsonProperty("frameType")]
    public int FrameType { get; init; }

    [JsonProperty("extended")]
    public Extended Extended { get; init; }

    [JsonProperty("craftedMods")]
    public IReadOnlyList<string> CraftedMods { get; init; }

    [JsonProperty("flavourText")]
    public IReadOnlyList<string> FlavourText { get; init; }

    [JsonProperty("corrupted")]
    public bool? Corrupted { get; init; }

    [JsonProperty("influences")]
    public Influences Influences { get; init; }

    [JsonProperty("shaper")]
    public bool? Shaper { get; init; }
}

public record Listing
{
    public Listing(string Method, DateTime Indexed, Stash Stash, string Whisper, Account Account, Price Price)
    {
        this.Method = Method;
        this.Indexed = Indexed;
        this.Stash = Stash;
        this.Whisper = Whisper;
        this.Account = Account;
        this.Price = Price;
    }

    [JsonProperty("method")]
    public string Method { get; init; }

    [JsonProperty("indexed")]
    public DateTime Indexed { get; init; }

    [JsonProperty("stash")]
    public Stash Stash { get; init; }

    [JsonProperty("whisper")]
    public string Whisper { get; init; }

    [JsonProperty("account")]
    public Account Account { get; init; }

    [JsonProperty("price")]
    public Price Price { get; init; }
}

public record Magnitude
{
    public Magnitude(string Hash, double Min, double Max)
    {
        this.Hash = Hash;
        this.Min = Min;
        this.Max = Max;
    }

    [JsonProperty("hash")]
    public string Hash { get; init; }

    [JsonProperty("min")]
    public double Min { get; init; }

    [JsonProperty("max")]
    public double Max { get; init; }
}

public record Mods
{
    public Mods(IReadOnlyList<Explicit> Explicit, IReadOnlyList<Implicit> Implicit, IReadOnlyList<Crafted> Crafted)
    {
        this.Explicit = Explicit;
        this.Implicit = Implicit;
        this.Crafted = Crafted;
    }

    [JsonProperty("explicit")]
    public IReadOnlyList<Explicit> Explicit { get; init; }

    [JsonProperty("implicit")]
    public IReadOnlyList<Implicit> Implicit { get; init; }

    [JsonProperty("crafted")]
    public IReadOnlyList<Crafted> Crafted { get; init; }
}

public record Price
{
    public Price(string Tag, string Type, double Amount, string Currency)
    {
        this.Tag = Tag;
        this.Type = Type;
        this.Amount = Amount;
        this.Currency = Currency;
    }

    [JsonProperty("tag")]
    public string Tag { get; init; }

    [JsonProperty("type")]
    public string Type { get; init; }

    [JsonProperty("amount")]
    public double Amount { get; init; }

    [JsonProperty("currency")]
    public string Currency { get; init; }
}

public record Property
{
    public Property(string Name, IReadOnlyList<List<JObject>> Values, int DisplayMode, int? Type)
    {
        this.Name = Name;
        this.Values = Values;
        this.DisplayMode = DisplayMode;
        this.Type = Type;
    }

    [JsonProperty("name")]
    public string Name { get; init; }

    [JsonProperty("values")]
    public IReadOnlyList<List<JObject>> Values { get; init; }

    [JsonProperty("displayMode")]
    public int DisplayMode { get; init; }

    [JsonProperty("type")]
    public int? Type { get; init; }
}

public record Requirement
{
    public Requirement(string Name, IReadOnlyList<List<JObject>> Values, int DisplayMode, int Type)
    {
        this.Name = Name;
        this.Values = Values;
        this.DisplayMode = DisplayMode;
        this.Type = Type;
    }

    [JsonProperty("name")]
    public string Name { get; init; }

    [JsonProperty("values")]
    public IReadOnlyList<List<JObject>> Values { get; init; }

    [JsonProperty("displayMode")]
    public int DisplayMode { get; init; }

    [JsonProperty("type")]
    public int Type { get; init; }
}

public record SearchResult
{
    public SearchResult(string Id, Listing Listing, Item Item)
    {
        this.Id = Id;
        this.Listing = Listing;
        this.Item = Item;
    }

    [JsonProperty("id")]
    public string Id { get; init; }

    [JsonProperty("listing")]
    public Listing Listing { get; init; }

    [JsonProperty("item")]
    public Item Item { get; init; }
}

public record SearchGetResult
{
    public SearchGetResult(IReadOnlyList<SearchResult> Results) { this.Results = Results; }

    [JsonProperty("result")]
    public IReadOnlyList<SearchResult> Results { get; init; }

    public SearchGetResult Join(SearchGetResult next)
    {
        var results = new List<SearchResult>(Results);
        results.AddRange(next.Results);
        return new SearchGetResult(results);
    }
}

public record Socket
{
    public Socket(int Group, string Attr, string SColour)
    {
        this.Group = Group;
        this.Attr = Attr;
        this.SColour = SColour;
    }

    [JsonProperty("group")]
    public int Group { get; init; }

    [JsonProperty("attr")]
    public string Attr { get; init; }

    [JsonProperty("sColour")]
    public string SColour { get; init; }
}

public record Stash
{
    public Stash(string Name, int X, int Y)
    {
        this.Name = Name;
        this.X = X;
        this.Y = Y;
    }

    [JsonProperty("name")]
    public string Name { get; init; }

    [JsonProperty("x")]
    public int X { get; init; }

    [JsonProperty("y")]
    public int Y { get; init; }
}

#endregion