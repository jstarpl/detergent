using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.Serialization;
using Detergent.RestClients;

namespace Detergent.Rest
{
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
    [Serializable]
    public class RestBadRequestException : RestClientException
    {
        public RestBadRequestException(Uri requestUrl, string message, string requestId) 
            : base(requestUrl, HttpStatusCode.BadRequest, message, requestId)
        {
        }

        protected RestBadRequestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}