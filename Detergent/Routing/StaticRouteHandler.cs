using System;

namespace Detergent.Routing
{
    public class StaticRouteHandler : IRouteHandler
    {
        public StaticRouteHandler(Func<IHttpContext, RouteMatchData, IHttpResponse> handlerFunc)
        {
            this.handlerFunc = handlerFunc;
        }

        public IHttpResponse Handle(IHttpContext context, RouteMatchData routeMatchData)
        {
            return handlerFunc(context, routeMatchData);
        }

        private readonly Func<IHttpContext, RouteMatchData, IHttpResponse> handlerFunc;
    }
}