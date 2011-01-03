using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Reflection;
using System.Text;
using log4net;

namespace Detergent
{
    public class DetergentHttpListener : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DetergentHttpListener"/> class using the specified parameters.
        /// </summary>
        /// <param name="uriPrefix">The global URI prefix for the application. Example: http://localhost:7777/mysite/</param>
        /// <param name="applicationPath">The path (name) of the Web application within the global prefix. Example: myapp</param>
        /// <param name="detergentHttpHandler">Specifies the <see cref="IDetergentHttpHandler"/> to use to handle HTTP requests.</param>
        [SuppressMessage ("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings")]
        public DetergentHttpListener (
            string uriPrefix,
            string applicationPath,
            IDetergentHttpHandler detergentHttpHandler)
        {
            if (uriPrefix == null)
                throw new ArgumentNullException("uriPrefix");
            if (applicationPath == null) 
                throw new ArgumentNullException("applicationPath");
            if (detergentHttpHandler == null) throw new ArgumentNullException("detergentHttpHandler");

            this.uriPrefix = uriPrefix;
            this.applicationPath = applicationPath;
            this.detergentHttpHandler = detergentHttpHandler;
            httpListener = new HttpListener();

            applicationRootUrl = new Uri(new Uri(uriPrefix), applicationPath).ToString();
            if (false == applicationRootUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase))
                applicationRootUrl = applicationRootUrl + '/';
            
            httpListener.Prefixes.Add(applicationRootUrl);

            DumpDiagnostics();

            httpListener.Start();
            IAsyncResult result = httpListener.BeginGetContext(WebRequestCallback, httpListener);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (false == disposed)
            {
                // clean native resources         

                if (disposing)
                {
                    // clean managed resources            
                    if (httpListener != null)
                    {
                        httpListener.Stop();
                        httpListener.Close();
                    }
                }

                disposed = true;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void WebRequestCallback(IAsyncResult result)
        {
            if (false == httpListener.IsListening)
                return;

            HttpListenerContext context = httpListener.EndGetContext(result);

            string requestId = IncidentIdGenerator.GenerateIncidentId();
            ThreadContext.Properties["requestId"] = requestId;

            httpListener.BeginGetContext(WebRequestCallback, httpListener);

            BufferedInputStream requestStream = new BufferedInputStream();
            requestStream.BufferStream(context.Request.InputStream);
            HttpListenerWrappedContext wrappedContext = new HttpListenerWrappedContext(
                context,
                requestStream,
                applicationRootUrl,
                applicationPath);
            CurrentHttpContext.Current = wrappedContext;
            wrappedContext.SetRequestId(requestId);

            if (log.IsDebugEnabled)
                log.DebugFormat("Received request: {0}", requestStream.DataToString(Encoding.UTF8));

            IHttpResponse response = null;
            try
            {
                response = detergentHttpHandler.ProcessRequest(wrappedContext);
            }
            catch (Exception ex)
            {
                log.Error("Error processing the request", ex);
                response = new ExceptionStackHttpResponse(HttpStatusCode.InternalServerError, ex);
                response.Send(wrappedContext);
                return;
            }

            try
            {
                if (response != null)
                    response.Send(wrappedContext);
            }
            catch (Exception ex)
            {
                log.Error("Could not send the response", ex);
                response = new ExceptionStackHttpResponse(HttpStatusCode.InternalServerError, ex);
                response.Send(wrappedContext);
            }
        }

        private void DumpDiagnostics()
        {
            if (log.IsDebugEnabled)
            {
                log.Debug("DetergentHttpListener diagnostics dump:");
                log.DebugFormat("   applicationPath='{0}'", applicationPath);
                log.DebugFormat("   applicationRootUrl='{0}'", applicationRootUrl);
                log.DebugFormat("   detergentHttpHandler='{0}'", detergentHttpHandler);
                log.DebugFormat("   uriPrefix='{0}'", uriPrefix);
                log.Debug("----------------");
                detergentHttpHandler.DumpDiagnostics();
            }
        }

        private readonly string applicationPath;
        private string applicationRootUrl;
        private readonly IDetergentHttpHandler detergentHttpHandler;
        private bool disposed;
        private readonly HttpListener httpListener;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        [SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        private readonly string uriPrefix;
    }
}