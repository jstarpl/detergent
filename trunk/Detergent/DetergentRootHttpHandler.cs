using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using Detergent.Rest;
using log4net;

namespace Detergent
{
    public class DetergentRootHttpHandler : IDetergentHttpHandler
    {
        public IDetergentRequestCleanup RequestCleanup
        {
            get { return requestCleanup; }
            set { requestCleanup = value; }
        }

        public IHttpResponse ProcessRequest(IHttpContext context)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat(
                    "Processing request (RequestUrl='{0}', RequestPath='{1}'", 
                    context.RequestUrl, 
                    context.RequestPath);
            }

            foreach (KeyValuePair<Regex, IDetergentHttpHandler> pair in subhandlers)
            {
                if (pair.Key.IsMatch(context.RequestPath))
                    return pair.Value.ProcessRequest(context);
            }

            return new RestErrorHttpResponse(
                HttpStatusCode.NotFound,
                "ROUTE NOT FOUND",
                "The request did not match any registered route.",
                String.Empty);
        }

        public void DumpDiagnostics()
        {
        }

        public DetergentRootHttpHandler AddSubhandler(string regexMatch, IDetergentHttpHandler handler)
        {
            subhandlers.Add(new Regex(regexMatch, RegexOptions.Compiled | RegexOptions.IgnoreCase), handler);
            return this;
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IDetergentRequestCleanup requestCleanup;
        private Dictionary<Regex, IDetergentHttpHandler> subhandlers = new Dictionary<Regex, IDetergentHttpHandler>();
    }
}