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
    Task<Response<ItemSearchPostResult>> PostSearch([Body] string query, [Path] string leagueName);

    [Get("/api/trade/fetch/{ids}")]
    Task<Response<SearchGetResult>> GetSearch([Path("ids")] string resultIds, [Query("query")] string queryId);

    [Get("/trade/search/{leagueName}/{queryId}")]
    Task<Response<string>> GetExistingSearch([Path] string leagueName, [Path] string queryId);
}

#region Exchange Results

public record ExchangeOffer
{
    public ExchangeOffer(string Id,
                         JObject Item,
                         ExchangeDetails Listing)
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
    public Online(string League,
                  string Status)
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
    public Account(string Name,
                   string LastCharacterName,
                   Online Online,
                   string Language)
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
    public Exchange(string Currency,
                    decimal Amount,
                    string Whisper)
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
    public ExchangeItem(string Currency,
                        decimal Amount,
                        int Stock,
                        string Id,
                        string Whisper)
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
    public ExchangeDetails(DateTime Indexed,
                           Account Account,
                           IReadOnlyList<Offer> Offers,
                           string Whisper)
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
    public Offer(Exchange Exchange,
                 ExchangeItem Item)
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

#region Search Results

public record ItemSearchPostResult
{
    public ItemSearchPostResult(string QueryId,
                            decimal Complexity,
                            IReadOnlyList<string> Results,
                            int Total)
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
    public Crafted(string Name,
                   string Tier,
                   int Level,
                   IReadOnlyList<Magnitude> Magnitudes)
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
    public Explicit(string Name,
                    string Tier,
                    int Level,
                    IReadOnlyList<Magnitude> Magnitudes)
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
    public Implicit(string Name,
                    string Tier,
                    int Level,
                    IReadOnlyList<Magnitude> Magnitudes)
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
    public Influences(bool Shaper)
    {
        this.Shaper = Shaper;
    }

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
    public Listing(string Method,
                   DateTime Indexed,
                   Stash Stash,
                   string Whisper,
                   Account Account,
                   Price Price)
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
    public Magnitude(string Hash,
                     double Min,
                     double Max)
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
    public Mods(IReadOnlyList<Explicit> Explicit,
                IReadOnlyList<Implicit> Implicit,
                IReadOnlyList<Crafted> Crafted)
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
    public Price(string Tag,
                 string Type,
                 double Amount,
                 string Currency)
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
    public Property(string Name,
                    IReadOnlyList<List<JObject>> Values,
                    int DisplayMode,
                    int? Type)
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
    public Requirement(string Name,
                       IReadOnlyList<List<JObject>> Values,
                       int DisplayMode,
                       int Type)
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
    public SearchResult(string Id,
                        Listing Listing,
                        Item Item)
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
    public SearchGetResult(IReadOnlyList<SearchResult> Results)
    {
        this.Results = Results;
    }

    [JsonProperty("result")]
    public IReadOnlyList<SearchResult> Results { get; init; }
}

public record Socket
{
    public Socket(int Group,
                  string Attr,
                  string SColour)
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
    public Stash(string Name,
                 int X,
                 int Y)
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