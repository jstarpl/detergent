using System;
using System.Diagnostics.CodeAnalysis;

namespace Detergent.RestClients
{
    public interface IRestClient : IDisposable
    {
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Call")]
        RestContent Call(RestRequest request);
        RestContent Send<T>(RestRequest request, T objectToSend)
            where T : class;
    }
}