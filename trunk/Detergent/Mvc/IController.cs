using Detergent.Routing;

namespace Detergent.Mvc
{
    public interface IController
    {
        void Initialize(IHttpContext httpContext, RouteMatchData routeMatchData);
    }
}