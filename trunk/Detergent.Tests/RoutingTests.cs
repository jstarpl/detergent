using Detergent.Routing;
using Detergent.Testing;
using MbUnit.Framework;

namespace Detergent.Tests
{
    public class RoutingTests
    {
        [Test]
        [Row(null)]
        [Row("name=something")]
        public void RouteIsMatch(string queryString)
        {
            Route route = new Route("product/(?<productId>[^//]+)/part/(?<partId>[^//]+)$");

            DummyHttpContext context = new DummyHttpContext(
                "http://google.com/service",
                "/product/10/part/20",
                queryString);
            RouteMatchData matchData = route.Match(context);

            Assert.IsNotNull(matchData);
            Assert.AreEqual("10", matchData.GetParameter("productId"));
            Assert.AreEqual("20", matchData.GetParameter("partId"));
        }

        [Test]
        public void RouteDoesNotMatch()
        {
            Route route = new Route("product/(?<productId>[^//]+)/part/(?<partId>[^//]+)$");

            DummyHttpContext context = new DummyHttpContext(
                "http://google.com/service/",
                "/service/product/10/part");
            RouteMatchData matchData = route.Match(context);

            Assert.IsNull(matchData);
        }
    }
}