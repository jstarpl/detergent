using System.Net;
using Detergent.Rest;
using Detergent.Routing;
using Microsoft.Http;

namespace Detergent.Tests
{
    public class SampleRestService
    {
        public void ConfigureRoutes(SimpleRestRequestRouter router)
        {
            router.AddRoute(HttpMethod.GET, "packages$", ListPackages);
            router.AddRoute(HttpMethod.GET, "package/(?<id>[^//]+)$", GetPackage);            
        }

        private static IHttpResponse GetPackage(IHttpContext context, RouteMatchData routeMatchData)
        {
            return LiteralHttpResponse.FormatHttpResponse(
                HttpStatusCode.OK,
                HttpConstants.ContentTypeText,
                "get{0}",
                routeMatchData.GetParameter("id"));
        }

        private static IHttpResponse ListPackages(IHttpContext context, RouteMatchData routeMatchData)
        {
            return LiteralHttpResponse.FormatHttpResponse(
                HttpStatusCode.OK, 
                HttpConstants.ContentTypeText,
                "list");
        }        
    }
}