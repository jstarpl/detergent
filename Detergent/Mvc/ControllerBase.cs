using System.Collections;
using System.IO;
using System.Text;
using Detergent.Routing;

namespace Detergent.Mvc
{
    public abstract class ControllerBase : IController
    {
        public void Initialize(
            IHttpContext httpContext, 
            RouteMatchData routeMatchData)
        {
            this.httpContext = httpContext;
            this.routeMatchData = routeMatchData;
        }

        protected ControllerBase(IFileCache fileCache)
        {
            this.fileCache = fileCache;
        }

        protected IFileCache FileCache
        {
            get { return fileCache; }
        }

        protected IHttpContext HttpContext
        {
            get { return httpContext; }
        }

        protected RouteMatchData RouteMatchData
        {
            get { return routeMatchData; }
        }

        protected IHttpResponse ReturnFile(string fileName, string contentType, Encoding encoding)
        {
            string fullFileName = Path.Combine(httpContext.ApplicationDirectoryPath, fileName);
            return new CachedFileResponse(fullFileName, fileCache, contentType, encoding);
        }

        protected IHttpResponse ReturnHtml(string templateFileName, Hashtable properties)
        {
            string fullFileName = Path.Combine(httpContext.ApplicationDirectoryPath, templateFileName);
            return MvcTemplateHttpResponse.ReturnHtml(fullFileName, fileCache, properties);
        }

        private IFileCache fileCache;
        private IHttpContext httpContext;
        private RouteMatchData routeMatchData;
    }
}