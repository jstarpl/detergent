using Detergent.Routing;

namespace Detergent.Rest
{
    public abstract class RestService : IRestService
    {
        public void Initialize(IHttpContext httpContext, RouteMatchData routeMatchData)
        {
            this.httpContext = httpContext;
            this.routeMatchData = routeMatchData;
        }

        protected IHttpContext HttpContext
        {
            get { return httpContext; }
        }

        protected RouteMatchData RouteMatchData
        {
            get { return routeMatchData; }
        }

        private IHttpContext httpContext;
        private RouteMatchData routeMatchData;
    }
}