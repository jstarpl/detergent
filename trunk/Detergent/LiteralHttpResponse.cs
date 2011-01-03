using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Text;

namespace Detergent
{
    public class LiteralHttpResponse : IHttpResponse
    {
        public LiteralHttpResponse(
            HttpStatusCode statusCode,
            string content, 
            string contentType)
        {
            this.statusCode = statusCode;
            this.content = content;
            this.contentType = contentType;
        }

        public string Content
        {
            get { return content; }
        }

        public string ContentType
        {
            get { return contentType; }
        }

        [SuppressMessage("Microsoft.Naming", "CA1719:ParameterNamesShouldNotMatchMemberNames", MessageId = "0#")]
        public static LiteralHttpResponse FormatHttpResponse(
            HttpStatusCode statusCode,
            string contentType,
            string format, 
            params object[] args)
        {
            string content = string.Format(
                CultureInfo.InvariantCulture,
                format,
                args);
            return new LiteralHttpResponse(
                statusCode,
                content,
                contentType);
        }

        public virtual void Send(IHttpContext context)
        {
            context.StatusCode = (int)statusCode;
            Encoding encoding = new UTF8Encoding(false);
            context.SetResponse(content, contentType, encoding);
        }

        private readonly HttpStatusCode statusCode;
        private string content;
        private readonly string contentType;
    }
}