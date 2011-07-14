using System.Text;
using Detergent.Soap;
using Detergent.Testing;
using MbUnit.Framework;

namespace Detergent.Tests
{
    public class SoapHttpHandlerTests
    {
        [Test]
        public void Test()
        {
            SimpleSoapRequestRouter router = new SimpleSoapRequestRouter();
            router.AddRoute(
                "display",
                "urn:mediator_platform_eInvoices_RetailGateway",
                (c, x) => new LiteralSoapEnvelope("response"));

            DetergentSoapHttpHandler handler = new DetergentSoapHttpHandler();
            handler.RequestRouter = router;

            DummyHttpContext httpContext = new DummyHttpContext();
            string requestContent = @"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:urn='urn:mediator_platform_eInvoices_RetailGateway'>
   <soapenv:Header/>
   <soapenv:Body>
      <urn:display>
         <!--Optional:-->
         <request>
            <invoiceID>?</invoiceID>
            <!--Zero or more repetitions:-->
            <properties name='?'>?</properties>
         </request>
      </urn:display>
   </soapenv:Body>
</soapenv:Envelope>";

            httpContext.SetRequestContent(
                "text/xml", 
                Encoding.UTF8,
                requestContent);
            LiteralSoapEnvelope envelope = (LiteralSoapEnvelope)handler.ProcessRequest(httpContext);
            Assert.AreEqual("response", envelope.Content);
        }
    }
}