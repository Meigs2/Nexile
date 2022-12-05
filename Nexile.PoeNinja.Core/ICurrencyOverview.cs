using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestEase;

namespace Nexile.PoeNinja.Core;

public interface ICurrencyOverview
{
    [Get("/api/data/CurrencyOverview")]
    [Header("Content-Type", "application/json")]
    Task<Response<CurrencyOverview>> Get([Query] string league, [Query] string type, [Query] string language = "en");
}
public record CurrencyDetail([property: JsonProperty("id")] int Id, [property: JsonProperty("icon")] string Icon,
    [property: JsonProperty("name")] string Name, [property: JsonProperty("tradeId")] string TradeId);

public record Language([property: JsonProperty("name")] string Name,
    [property: JsonProperty("translations")] object Translations);

public record Line([property: JsonProperty("currencyTypeName")] string CurrencyTypeName,
    [property: JsonProperty("pay")] Pay Pay, [property: JsonProperty("receive")] Receive Receive,
    [property: JsonProperty("paySparkLine")] PaySparkLine PaySparkLine,
    [property: JsonProperty("receiveSparkLine")] ReceiveSparkLine ReceiveSparkLine,
    [property: JsonProperty("chaosEquivalent")] decimal ChaosEquivalent,
    [property: JsonProperty("lowConfidencePaySparkLine")] LowConfidencePaySparkLine LowConfidencePaySparkLine,
    [property: JsonProperty("lowConfidenceReceiveSparkLine")]
    LowConfidenceReceiveSparkLine LowConfidenceReceiveSparkLine,
    [property: JsonProperty("detailsId")] string DetailsId);

public record LowConfidencePaySparkLine([property: JsonProperty("data")] IReadOnlyList<decimal?> Data,
    [property: JsonProperty("totalChange")] decimal TotalChange);

public record LowConfidenceReceiveSparkLine([property: JsonProperty("data")] IReadOnlyList<decimal> Data,
    [property: JsonProperty("totalChange")] decimal TotalChange);

public record Pay([property: JsonProperty("id")] int Id, [property: JsonProperty("league_id")] int LeagueId,
    [property: JsonProperty("pay_currency_id")] int PayCurrencyId,
    [property: JsonProperty("get_currency_id")] int GetCurrencyId,
    [property: JsonProperty("sample_time_utc")] DateTime SampleTimeUtc, [property: JsonProperty("count")] int Count,
    [property: JsonProperty("value")] decimal Value, [property: JsonProperty("data_point_count")] int DataPointCount,
    [property: JsonProperty("includes_secondary")] bool IncludesSecondary,
    [property: JsonProperty("listing_count")] int ListingCount);

public record PaySparkLine([property: JsonProperty("data")] IReadOnlyList<object> Data,
    [property: JsonProperty("totalChange")] decimal TotalChange);

public record Receive([property: JsonProperty("id")] int Id, [property: JsonProperty("league_id")] int LeagueId,
    [property: JsonProperty("pay_currency_id")] int PayCurrencyId,
    [property: JsonProperty("get_currency_id")] int GetCurrencyId,
    [property: JsonProperty("sample_time_utc")] DateTime SampleTimeUtc, [property: JsonProperty("count")] int Count,
    [property: JsonProperty("value")] decimal Value, [property: JsonProperty("data_point_count")] int DataPointCount,
    [property: JsonProperty("includes_secondary")] bool IncludesSecondary,
    [property: JsonProperty("listing_count")] int ListingCount);

public record ReceiveSparkLine([property: JsonProperty("data")] IReadOnlyList<decimal> Data,
    [property: JsonProperty("totalChange")] decimal TotalChange);

public record CurrencyOverview([property: JsonProperty("lines")] IReadOnlyList<Line> Lines,
    [property: JsonProperty("currencyDetails")] IReadOnlyList<CurrencyDetail> CurrencyDetails,
    [property: JsonProperty("language")] Language Language);