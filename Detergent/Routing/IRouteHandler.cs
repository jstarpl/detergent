namespace Detergent.Routing
{
    public interface IRouteHandler
    {
        IHttpResponse Handle(IHttpContext context, RouteMatchData routeMatchData);
    }
}