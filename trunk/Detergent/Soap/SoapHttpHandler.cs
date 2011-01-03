using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Web;
using System.Xml;
using log4net;

namespace Detergent.Soap
{
    public class SoapHttpHandler : IHttpHandler
    {
        public const string SoapHttpHandlerRouterKeyId = "SoapHttpHandlerRouter";

        public SoapHttpHandler()
        {
            log.Info("SoapHttpHandler created");
        }

        public bool IsReusable
        {
            get { return true; }
        }

        public ISoapRequestRouter RequestRouter
        {
            get { return requestRouter; }
            set { requestRouter = value; }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void ProcessRequest(HttpContext context)
        {
            FetchRouterFromAppCache(context);

            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Request raw URL: '{0}'", context.Request.RawUrl);
                log.DebugFormat("Request content encoding: '{0}'", context.Request.ContentEncoding);
                log.DebugFormat("Request content length: {0}", context.Request.ContentLength);
                log.DebugFormat("Request content type: '{0}'", context.Request.ContentType);
            }

            string content = null;
            using (StreamReader reader = new StreamReader(
                context.Request.InputStream,
                context.Request.ContentEncoding))
                content = reader.ReadToEnd();

            if (log.IsDebugEnabled)
                log.DebugFormat("Request content: '{0}'", content);

            WebHttpContext soapContext = new WebHttpContext(context);

            try
            {
                ISoapEnvelope responseEnvelope = ProcessRequest(soapContext, content);
                soapContext.Respond(responseEnvelope);
            }
            catch (SoapClientException ex)
            {
                soapContext.RespondWithFault(SoapFaultCode.Client, ex.Message);
            }
            catch (Exception ex)
            {
                log.Error("Unhandled error processing request", ex);
                soapContext.RespondWithFault(SoapFaultCode.Server, "Internal server error");
            }
        }

        public ISoapEnvelope ProcessRequest(IHttpContext httpContext, string content)
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
                return ProcessRequest(httpContext, node);
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

        private void FetchRouterFromAppCache(HttpContext context)
        {
            if (requestRouter == null)
                requestRouter = (ISoapRequestRouter) context.Application[SoapHttpHandlerRouterKeyId];
        }

        private ISoapEnvelope ProcessRequest(IHttpContext httpContext, XmlNode requestNode)
        {
            if (requestRouter == null)
                return new SoapEnvelope().Fault(
                    SoapFaultCode.Server, 
                    "Web service is not properly configured");

            ISoapEnvelope response = requestRouter.RouteSoapRequest(httpContext, requestNode);
            if (response != null)
                return response;

            return new SoapEnvelope().Fault(SoapFaultCode.Client, "Method not supported");
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(SoapHttpHandler));
        private ISoapRequestRouter requestRouter;
    }
}