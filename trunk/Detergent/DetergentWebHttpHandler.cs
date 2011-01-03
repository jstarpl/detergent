using System.Configuration;
using System.Web;

namespace Detergent
{
    public class DetergentWebHttpHandler : IHttpHandler
    {
        public const string DetergentWebHttpHandlerId = "DetergentWebHttpHandlerId";

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string requestId = IncidentIdGenerator.GenerateIncidentId();
            log4net.ThreadContext.Properties["requestId"] = requestId;

            internalHttpHttpHandler = (IDetergentHttpHandler)context.Application[DetergentWebHttpHandlerId];

            if (internalHttpHttpHandler == null)
                throw new ConfigurationErrorsException("DetergentWebHttpHandler does not have a configured inner handler");

            WebHttpContext contextWrapper = new WebHttpContext(context);
            CurrentHttpContext.Current = contextWrapper;
            contextWrapper.SetRequestId(requestId);

            IHttpResponse response = internalHttpHttpHandler.ProcessRequest(contextWrapper);
            if (response != null)
                response.Send(contextWrapper);
        }

        private IDetergentHttpHandler internalHttpHttpHandler;
    }
}