using System.Net;
using System.Threading;
using Detergent.Soap;
using MbUnit.Framework;
using Rhino.Mocks;

namespace Detergent.Tests
{
    public class EmbeddedWebServerTests
    {
        [Test]
        public void Test()
        {
            httpHandler.Stub(x => x.ProcessRequest(null)).IgnoreArguments()
                .Return(new ErrorSoapEnvelope(SoapFaultCode.VersionMismatch, "test"));

            for (int i = 0; i < 5; i++)
            {
                using (WebClient webClient = new WebClient())
                {
                    string response = webClient.DownloadString("http://localhost:9999/test/");
                    Assert.IsFalse(string.IsNullOrEmpty(response));
                    Assert.IsTrue(response.StartsWith("<soap"));
                }

                Thread.Sleep(100);
            }

            //Thread.Sleep(5000);
        }

        [SetUp]
        private void Setup()
        {
            httpHandler = MockRepository.GenerateMock<IDetergentHttpHandler>();
            listener = new DetergentHttpListener(
                "http://localhost:9999",
                "/test",
                httpHandler);
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
        private IDetergentHttpHandler httpHandler;
    }
}