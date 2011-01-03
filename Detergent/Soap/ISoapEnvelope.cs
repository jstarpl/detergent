using System.Diagnostics.CodeAnalysis;

namespace Detergent.Soap
{
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces")]
    public interface ISoapEnvelope : IHttpResponse
    {
    }
}