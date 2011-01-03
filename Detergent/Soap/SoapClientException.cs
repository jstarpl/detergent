using System;
using System.Runtime.Serialization;

namespace Detergent.Soap
{
    [Serializable]
    public class SoapClientException : Exception
    {
        public SoapClientException()
        {
        }

        public SoapClientException(string message) : base(message)
        {
        }

        public SoapClientException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SoapClientException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}