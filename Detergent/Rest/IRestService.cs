using System;
using Detergent.Routing;

namespace Detergent.Rest
{
    public interface IRestService
    {
        void Initialize(IHttpContext httpContext, RouteMatchData routeMatchData);
    }
}