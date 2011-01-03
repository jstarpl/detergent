using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Reflection;
using Detergent.Rest;
using log4net;

namespace Detergent.Mvc
{
    public class DetergentMvcHttpHandler : IDetergentHttpHandler
    {
        public const string DetergentRestHttpHandlerRouterKeyId = "DetergentRestHttpHandlerRouterKeyId";

        public MvcRequestRouter RequestRouter
        {
            get { return requestRouter; }
            set { requestRouter = value; }
        }

        public IDetergentRequestCleanup RequestCleanup
        {
            get { return requestCleanup; }
            set { requestCleanup = value; }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public IHttpResponse ProcessRequest(IHttpContext context)
        {
            try
            {
                return requestRouter.RouteRestRequest(context);
            }
            catch (RestBadRequestException ex)
            {
                if (log.IsInfoEnabled)
                    log.Info("ProcessRequest", ex);

                return new MvcErrorHttpResponse(
                    HttpStatusCode.BadRequest,
                    null,
                    "The request could not be recognized");
            }
            catch (Exception ex)
            {
                if (log.IsInfoEnabled)
                    log.InfoFormat("ProcessRequest ('{0}') exception: {1}", context.RequestUrl, ex);

                return new MvcErrorHttpResponse(
                    HttpStatusCode.InternalServerError,
                    null,
                    ex.ToString());
            }
        }

        public void DumpDiagnostics()
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("DetergentMvcHttpHandler diagnostics dump:");
                log.DebugFormat("   requestRouter='{0}'", requestRouter);
                log.Debug("----------------");
            }
        }

        private MvcRequestRouter requestRouter;
        private IDetergentRequestCleanup requestCleanup;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}