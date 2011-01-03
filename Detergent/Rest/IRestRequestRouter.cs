namespace Detergent.Rest
{
    public interface IRestRequestRouter
    {
        IHttpResponse RouteRestRequest(IHttpContext context);
    }
}