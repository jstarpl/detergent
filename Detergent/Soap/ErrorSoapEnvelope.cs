using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

namespace Detergent.Soap
{
    public class ErrorSoapEnvelope : ISoapEnvelope
    {
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string")]
        public ErrorSoapEnvelope(SoapFaultCode faultCode, string faultString)
        {
            content = string.Format(
                CultureInfo.InvariantCulture,
                ErrorSoapContent,
                faultCode,
                faultString);
         }

        public void Send(IHttpContext context)
        {
            Encoding encoding = new UTF8Encoding(false);
            context.SetResponse(content, HttpConstants.ContentTypeXml, encoding);
        }

        private const string ErrorSoapContent = @"<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:urn='urn:mediator_platform_eInvoices_RetailGateway'>
   <soapenv:Body>
      <soapenv:Fault>
         <faultcode>soapenv:{0}</faultcode>
         <faultstring>soapenv:{1}</faultstring>
      </soapenv:Fault>
   </soapenv:Body>
</soapenv:Envelope>";
        private readonly string content;
    }
}