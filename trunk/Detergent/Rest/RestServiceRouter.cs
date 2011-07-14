using System;
using System.Collections.Generic;
using Detergent.Routing;
using Microsoft.Http;

namespace Detergent.Rest
{
    public class RestServiceRouter : IRestRequestRouter
    {
        public RestServiceRouter(IRestServicesRegistry servicesRegistry)
        {
            this.servicesRegistry = servicesRegistry;
        }

        public void AddRoute<TService>(
            HttpMethod httpMethod, 
            string urlScheme, 
            Func<TService, IHttpResponse> serviceMethod)
            where TService : IRestService
        {
            servicesRegistry.EnsureIsRegistered<TService>();

            routes.Add(
                new Route(httpMethod, urlScheme),
                new ServiceRouteHandler<TService>(servicesRegistry, serviceMethod));
        }

        public IEnumerable<KeyValuePair<Route, IRouteHandler>> EnumerateRoutes()
        {
            return routes;
        }

        public IHttpResponse RouteRestRequest(IHttpContext context)
        {
            foreach (KeyValuePair<Route, IRouteHandler> pair in routes)
            {
                RouteMatchData matchData = pair.Key.Match(context);
                if (matchData == null)
                    continue;

                return pair.Value.Handle(context, matchData);
            }

            throw new RestBadRequestException(new Uri(context.RequestUrl), null, null);
        }

        private Dictionary<Route, IRouteHandler> routes =
            new Dictionary<Route, IRouteHandler>();
        private readonly IRestServicesRegistry servicesRegistry;
    }
}