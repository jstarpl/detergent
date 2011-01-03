using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Reflection;
using log4net;

namespace Detergent.Rest
{
    public class DetergentRestHttpHandler : IDetergentHttpHandler
    {
        public const string DetergentRestHttpHandlerRouterKeyId = "DetergentRestHttpHandlerRouterKeyId";

        public IRestRequestRouter RequestRouter
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
                FetchRouterFromAppCache(context);

                return requestRouter.RouteRestRequest(context);
            }
            catch (RestBadRequestException)
            {
                return new RestErrorHttpResponse(
                    HttpStatusCode.BadRequest,
                    null,
                    "The request could not be recognized",
                    context.RequestId);
            }
            catch (Exception ex)
            {
                log.Error("ProcessRequest failed", ex);

                return new RestErrorHttpResponse(
                    HttpStatusCode.InternalServerError,
                    "InternalServerError",
                    null,
                    context.RequestId);
            }
            finally
            {
                if (requestCleanup != null)
                    requestCleanup.AfterRequest(context);
            }
        }

        public void DumpDiagnostics()
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("DetergentRestHttpHandler diagnostics dump:");
                log.DebugFormat("   requestRouter='{0}'", requestRouter);
                log.Debug("----------------");
            }
        }

        private void FetchRouterFromAppCache(IHttpContext context)
        {
            if (requestRouter == null)
                requestRouter = (IRestRequestRouter)context.GetAppCacheValue(DetergentRestHttpHandlerRouterKeyId);

            if (requestRouter == null)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} is missing a router, you need to specify it in Web application cache under the key '{1}'.",
                    typeof(DetergentRestHttpHandler).Name,
                    DetergentRestHttpHandlerRouterKeyId);
                throw new InvalidOperationException(message);
            }
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IRestRequestRouter requestRouter;
        private IDetergentRequestCleanup requestCleanup;
    }
}