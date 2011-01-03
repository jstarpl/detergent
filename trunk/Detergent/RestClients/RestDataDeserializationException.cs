using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Detergent.RestClients
{
    [Serializable]
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
    public class RestDataDeserializationException : RestException
    {
        public RestDataDeserializationException(
            Uri requestUrl, 
            string message, 
            Exception inner) 
            : base(requestUrl, message, inner)
        {
        }

        protected RestDataDeserializationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}