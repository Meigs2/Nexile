using Nexile.PathOfExile.QueryBuilder;
using Nexile.Tests.Common;

namespace Nexile.Core.Tests.Unit.Services.PathOfExile.QueryBuilder;

public class StatFilterTests : QueryBuilderTestFixture
{
    [Test]
    public Task Creates_Default_Trade_Query_By_Default()
    {
        var tradeQuery = new TradeQuery();
        tradeQuery.Log();
        return Verify(tradeQuery);
    }
}