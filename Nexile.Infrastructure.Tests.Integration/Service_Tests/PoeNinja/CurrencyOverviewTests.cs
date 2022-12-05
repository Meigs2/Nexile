using Microsoft.Extensions.DependencyInjection;
using Nexile.PoeNinja.Core;
using Nexile.Tests.Common;

namespace Nexile.Infrastructure.Tests.Integration.Service_Tests.PoeNinja;

[TestFixture]
public class CurrencyOverviewTests : RateLimitedServiceTestFixture
{
    private ICurrencyOverview _currencyOverview;

    [SetUp]
    public void Setup()
    {
        _currencyOverview = ServiceProvider.GetService<ICurrencyOverview>() ?? throw new InvalidOperationException();
    }

    [Test]
    public async Task Can_Get_Trade_Overview()
    {
        using var response = await _currencyOverview.Get("Standard", "Currency");
        
        response.StringContent.Log();
    }
}