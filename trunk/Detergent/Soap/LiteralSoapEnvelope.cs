using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;

namespace Detergent.Soap
{
    public class LiteralSoapEnvelope : LiteralHttpResponse, ISoapEnvelope
    {
        public LiteralSoapEnvelope(string content) : base(HttpStatusCode.OK, content, HttpConstants.ContentTypeXml)
        {
        }

        [SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#")]
        public static LiteralSoapEnvelope FormatSoapEnvelope(string format, params object[] args)
        {
            return new LiteralSoapEnvelope(string.Format(
                                               CultureInfo.InvariantCulture,
                                               format,
                                               args));
        }
    }
}