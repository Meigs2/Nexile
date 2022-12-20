using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Nexile.PathOfExile;
using Nexile.Tests.Common;

namespace Nexile.Infrastructure.Tests.Integration.Service_Tests.Official_Poe_Api;

[TestFixture]
public class Exchange_Api : RateLimitedServiceTestFixture
{
    private PathOfExileApi _poeService = null!;

    [SetUp]
    public void Setup()
    {
        _poeService = ServiceProvider.GetService<PathOfExileApi>() ?? throw new InvalidOperationException();
    }

    private ExchangeQuery DivToChaosExchange = new()
    {
        QueryString =
            "{\"query\":{\"status\":{\"option\":\"online\"},\"have\":[\"divine\"],\"want\":[\"chaos\"]},\"sort\":{\"have\":\"asc\"},\"engine\":\"new\"}",
        LeagueName = "Standard"
    };

    [Test]
    public async Task Can_Search_Exchange()
    {
        var response = await _poeService.SearchExchange(DivToChaosExchange);
        response.IsSuccess.Should().BeTrue();
        response.Log();
    }

    public static ItemSearchQuery ClawQuery = new()
    {
        QueryString =
            "{\"query\":{\"status\":{\"option\":\"online\"},\"type\":\"Imperial Claw\",\"stats\":[{\"type\":\"and\",\"filters\":[],\"disabled\":false}]},\"sort\":{\"price\":\"asc\"}}",
        LeagueName = "Standard"
    };

    public static TradeSearch ClawQuerySearch = new() { OriginalQuery = ClawQuery, Complexity = 4, QueryId = "m4kwH6" };

    [Test]
    public async Task Can_Create_Item_Search()
    {
        var result = await _poeService.CreateItemSearch(ClawQuery);
        result.IsSuccess.Should().BeTrue();
        result.Log();
    }

    [Test]
    public async Task Can_Search_Existing_Query()
    {
        var queryString = await _poeService.QueryExistingSearch(ClawQuerySearch);
        queryString.IsSuccess.Should().BeTrue();
        queryString.Log();
    }

    [Test]
    public async Task Can_Get_Items_From_Search_Result()
    {
        var search = await _poeService.CreateItemSearch(ClawQuery);
        search.IsSuccess.Should().BeTrue();
        search.Log();
        
        var result = await _poeService.GetItemSearchResults(search.Value);
        result.IsSuccess.Should().BeTrue();
        result.Log();
    }
}