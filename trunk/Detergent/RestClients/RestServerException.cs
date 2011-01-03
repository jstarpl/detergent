using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.Serialization;

namespace Detergent.RestClients
{
    [SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly")]
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
    [Serializable]
    public class RestServerException : RestException
    {
        public RestServerException(Uri requestUrl, HttpStatusCode httpStatusCode, string message, string requestId, Exception inner) : base(requestUrl, httpStatusCode, message, requestId, inner)
        {
        }

        public RestServerException(Uri requestUrl, HttpStatusCode httpStatusCode, string message, string requestId) : base(requestUrl, httpStatusCode, message, requestId)
        {
        }

        public RestServerException(Uri requestUrl, string message, Exception inner) : base(requestUrl, message, inner)
        {
        }

        protected RestServerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}