using System.Net;
using System.Threading;
using Detergent.Rest;
using MbUnit.Framework;

namespace Detergent.Tests
{
    public class EmbeddedRestServerTests
    {
        [Test]
        public void Test()
        {
            for (int i = 0; i < 5; i++)
            {
                using (WebClient webClient = new WebClient())
                {
                    string response = webClient.DownloadString(
                        "http://localhost:9999/service/packages#fragment?page=2");
                    Assert.IsFalse(string.IsNullOrEmpty(response));
                    Assert.AreEqual("list", response);
                }

                Thread.Sleep(100);
            }
        }

        [SetUp]
        private void Setup()
        {
            httpHandler = new DetergentRestHttpHandler();
            listener = new DetergentHttpListener(
                "http://localhost:9999",
                "/service",
                httpHandler);

            SimpleRestRequestRouter router = new SimpleRestRequestRouter();
            httpHandler.RequestRouter = router;

            SampleRestService service = new SampleRestService();
            service.ConfigureRoutes(router);
        }

        [TearDown]
        private void Teardown()
        {
            if (listener != null)
            {
                listener.Dispose();
                listener = null;
            }
        }

        private DetergentHttpListener listener;
        private DetergentRestHttpHandler httpHandler;
    }
}