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

    [Test]
    public async Task Can_Post_Exchange()
    {
        var response = await _poeService.PostExchange(
            "{\"query\":{\"status\":{\"option\":\"online\"},\"have\":[\"divine\"],\"want\":[\"chaos\"]},\"sort\":{\"have\":\"asc\"},\"engine\":\"new\"}",
            "Standard");
        response.IsSuccess.Should().BeTrue();
        response.Log();
    }

    [Test]
    public async Task Can_Post_And_Get_Results_Search()
    {
        var response = await _poeService.PostSearch(
            "{\"query\":{\"status\":{\"option\":\"online\"},\"type\":\"Imperial Claw\",\"stats\":[{\"type\":\"and\",\"filters\":[],\"disabled\":false}]},\"sort\":{\"price\":\"asc\"}}",
            "Standard");
        response.IsSuccess.Should().BeTrue();
        response.Log();
    }

    [Test]
    public async Task Can_Get_Existing_Query_From_Query_Id()
    {
        var response = await _poeService.GetExistingSearchByQueryId("Standard", "E4QrQLu5");
        response.IsSuccess.Should().BeTrue();
        response.Log();
    }

    [Test]
    public async Task Can_Subscribe_And_Receive_Live_Search_Results()
    {
        var success = false;
        _poeService.LiveSearchEvents.Subscribe(e => e.Log());
        var result = await _poeService.SubscribeLiveSearch("Kalandra", "LgvZCn");
        result.IsSuccess.Should().BeTrue();
        result.Log();
    }
    
    [Test]
    public async Task Can_Unsubscribe_From_Live_Search()
    {
        var success = false;
        var search = ("Kalandra", "LgvZCn");
        var @event = new ManualResetEvent(false);
        _poeService.LiveSearchEvents.Subscribe(e => e.Log());
        var result = await _poeService.SubscribeLiveSearch(search.Item1, search.Item2);
        result.IsSuccess.Should().BeTrue();
        result.Value.Subscribe(e =>
        {
            e.Log();
            success = _poeService.UnsubscribeLiveSearch(search.Item1, search.Item2).Result;
            success.Log();
            @event.Set();
        });
        @event.WaitOne(10000);
        success.Should().BeTrue();
    }
}