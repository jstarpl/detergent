using System.Net;
using Detergent.Rest;
using MbUnit.Framework;
using Microsoft.Http;

namespace Detergent.Tests
{
    public class RestHttpHandlerTests
    {
        [Test]
        [Row(@"/packages", null)]
        [Row(@"/packages", "page=2")]
        public void TestListPackages(string path, string queryString)
        {
            context = new DummyHttpContext(
                "http://google.com/service",
                path,
                queryString);
            context.RequestHttpMethod = "GET";

            IHttpResponse response = handler.ProcessRequest(context);
            LiteralHttpResponse literal = (LiteralHttpResponse)response;
            Assert.AreEqual("list", literal.Content);
        }

        [Test]
        public void TestListPackagesWithWrongMethod()
        {
            context = new DummyHttpContext(
                "http://google.com/service",
                "/packages");
            context.RequestHttpMethod = HttpMethod.POST.ToString();

            IHttpResponse response = handler.ProcessRequest(context);
            RestErrorHttpResponse errorResponse = (RestErrorHttpResponse)response;
            Assert.AreEqual((int)HttpStatusCode.BadRequest, errorResponse.HttpStatusCode);
        }

        [Test]
        public void TestGetPackage()
        {
            context = new DummyHttpContext(
                "http://google.com/service",
                "/package/20");
            context.RequestHttpMethod = "GET";

            IHttpResponse response = handler.ProcessRequest(context);
            LiteralHttpResponse literal = (LiteralHttpResponse)response;
            Assert.AreEqual("get20", literal.Content);
        }

        [SetUp]
        private void Setup()
        {
            handler = new DetergentRestHttpHandler();
            SimpleRestRequestRouter router = new SimpleRestRequestRouter();
            handler.RequestRouter = router;

            SampleRestService service = new SampleRestService();
            service.ConfigureRoutes(router);
        }

        private DetergentRestHttpHandler handler;
        private DummyHttpContext context;
    }
}