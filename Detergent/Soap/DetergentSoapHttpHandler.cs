using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using log4net;

namespace Detergent.Soap
{
    public class DetergentSoapHttpHandler : IDetergentHttpHandler
    {
        public const string DetergentSoapHttpHandlerRouterKeyId = "DetergentSoapHttpHandlerRouterKeyId";

        public DetergentSoapHttpHandler()
        {
            log.Info("DetergentSoapHttpHandler created");
        }

        public ISoapRequestRouter RequestRouter
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

                if (log.IsDebugEnabled)
                {
                    log.DebugFormat("Request URL: '{0}'", context.RequestUrl);
                    log.DebugFormat("Request content encoding: '{0}'", context.RequestContentEncoding);
                    log.DebugFormat("Request content length: {0}", context.RequestContentLength);
                    log.DebugFormat("Request content type: '{0}'", context.RequestContentType);
                }

                string content = null;
                using (StreamReader reader = new StreamReader(
                    context.RequestInputStream,
                    context.RequestContentEncoding))
                    content = reader.ReadToEnd();

                if (log.IsDebugEnabled)
                    log.DebugFormat("Request content: '{0}'", content);

                try
                {
                    return ProcessSoapRequest(context, content);
                }
                catch (SoapClientException ex)
                {
                    return new ErrorSoapEnvelope(SoapFaultCode.Client, ex.Message);
                }
                catch (Exception ex)
                {
                    log.Error("Unhandled error processing request", ex);
                    return new ErrorSoapEnvelope(SoapFaultCode.Server, "Internal server error");
                }
            }
            finally
            {
                if (requestCleanup != null)
                    requestCleanup.AfterRequest(context);
            }
        }

        public ISoapEnvelope ProcessSoapRequest(IHttpContext httpContext, string content)
        {
            try
            {
                if (String.IsNullOrEmpty(content))
                    throw new SoapClientException("Missing SOAP request envelope");

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(content);

                XmlElement bodyElement = xmlDocument.DocumentElement["Body", SoapConstants.SoapEnvelopeNS];
                if (bodyElement == null)
                    throw new SoapClientException("Missing SOAP body element");

                if (bodyElement.ChildNodes.Count == 0)
                    throw new SoapClientException("SOAP body element has no children");

                XmlNode node = bodyElement.ChildNodes[0];
                return ProcessSoapRequest(httpContext, node);
            }
            catch (SoapClientException)
            {
                throw;
            }
            catch (Exception ex)
            {
                //throw new SoapClientException(ex.Message);
                throw new SoapClientException("Could not read the SOAP request", ex);
            }
        }

        public void DumpDiagnostics()
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("DetergentSoapHttpHandler diagnostics dump:");
                log.DebugFormat("   requestRouter='{0}'", requestRouter);
                log.Debug("----------------");
            }
        }

        private void FetchRouterFromAppCache(IHttpContext context)
        {
            if (requestRouter == null)
                requestRouter = (ISoapRequestRouter)context.GetAppCacheValue(DetergentSoapHttpHandlerRouterKeyId);
        }

        private ISoapEnvelope ProcessSoapRequest(IHttpContext httpContext, XmlNode requestNode)
        {
            if (requestRouter == null)
                return new ErrorSoapEnvelope(
                    SoapFaultCode.Server, 
                    "Web service is not properly configured");

            ISoapEnvelope response = requestRouter.RouteSoapRequest(httpContext, requestNode);
            if (response != null)
                return response;

            return new ErrorSoapEnvelope(SoapFaultCode.Client, "Method not supported");
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(DetergentSoapHttpHandler));
        private ISoapRequestRouter requestRouter;
        private IDetergentRequestCleanup requestCleanup;
    }
}