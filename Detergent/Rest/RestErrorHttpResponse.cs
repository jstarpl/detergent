using System.Net;
using Detergent.Templating;

namespace Detergent.Rest
{
    public class RestErrorHttpResponse : TemplateHttpResponse
    {
        public RestErrorHttpResponse(int httpStatusCode, string errorId, string message, string incidentId)
        {
            this.httpStatusCode = httpStatusCode;
            this.errorId = errorId;
            this.message = message;
            Template = new StringTemplate(ErrorResponseTemplateText);
            this.incidentId = incidentId;
        }

        public RestErrorHttpResponse(HttpStatusCode httpStatusCode, string errorId, string message, string incidentId)
            : this((int)httpStatusCode, errorId, message, incidentId)
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

        public string IncidentId
        {
            get
            {
                return incidentId;
            }
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
            Properties["incidentId"] = incidentId;
            base.Send(context);
        }

        private const string ErrorResponseTemplateText = @"<error>
    <message><![CDATA[$message]]></message>
    <error-id>$!errorId</error-id>
    <incident-id>$!incidentId</incident-id>
</error>";

        private readonly int httpStatusCode;
        private readonly string errorId;
        private readonly string message;
        private readonly string incidentId;
    }
}