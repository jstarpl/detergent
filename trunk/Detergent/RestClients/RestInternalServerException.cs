using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.Serialization;

namespace Detergent.RestClients
{
    [Serializable]
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
    public class RestInternalServerException : RestServerException
    {
        public RestInternalServerException(
            Uri requestUrl, 
            HttpStatusCode httpStatusCode, 
            string message, 
            string requestId) 
            : base(requestUrl, httpStatusCode, message, requestId)
        {
        }

        protected RestInternalServerException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}