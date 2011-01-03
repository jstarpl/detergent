using System;
using System.Runtime.Serialization;

namespace Detergent.Mime
{
    [Serializable]
    public class MultipartMessageParserException : Exception
    {
        public MultipartMessageParserException()
        {
        }

        public MultipartMessageParserException(string message) : base(message)
        {
        }

        public MultipartMessageParserException(string message, Exception inner) : base(message, inner)
        {
        }

        protected MultipartMessageParserException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}