using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace Detergent.Soap
{
    public interface ISoapRequestRouter
    {
        [SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
        ISoapEnvelope RouteSoapRequest(IHttpContext httpContext, XmlNode requestNode);
    }
}