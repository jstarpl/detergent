using System.Net;
using Detergent.Templating;

namespace Detergent.Mvc
{
    public class MvcErrorHttpResponse : TemplateHttpResponse
    {
        public MvcErrorHttpResponse(int httpStatusCode, string errorId, string message)
        {
            this.httpStatusCode = httpStatusCode;
            this.errorId = errorId;
            this.message = message;
            Template = new StringTemplate(ErrorResponseTemplateText);
        }

        public MvcErrorHttpResponse(HttpStatusCode httpStatusCode, string errorId, string message)
            : this((int)httpStatusCode, errorId, message)
        {
        }

        public int HttpStatusCode
        {
            get { return httpStatusCode; }
        }

        public string ErrorId
        {
            get { return errorId; }
        }

        public string Message
        {
            get { return message; }
        }

        public override void Send(IHttpContext context)
        {
            context.StatusCode = httpStatusCode;
            Properties["message"] = message;
            Properties["errorId"] = errorId;
            base.Send(context);
        }

        private const string ErrorResponseTemplateText = @"$message";

        private readonly int httpStatusCode;
        private readonly string errorId;
        private readonly string message;
    }
}