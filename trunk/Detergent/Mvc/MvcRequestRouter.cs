using System;
using System.Collections.Generic;
using Detergent.Rest;
using Detergent.Routing;
using Microsoft.Http;

namespace Detergent.Mvc
{
    public class MvcRequestRouter : IRestRequestRouter
    {
        public MvcRequestRouter(IControllerRegistry controllerRegistry)
        {
            this.controllerRegistry = controllerRegistry;
        }

        public void AddRoute<TController>(
            HttpMethod httpMethod,
            string urlScheme,
            Func<TController, IHttpResponse> controllerAction)
            where TController : class, IController
        {
            controllerRegistry.EnsureIsRegistered<TController>();

            routes.Add(
                new Route(httpMethod, urlScheme),
                new ControllerRouteHandler<TController>(controllerRegistry, controllerAction));
        }

        public IHttpResponse RouteRestRequest(IHttpContext context)
        {
            foreach (KeyValuePair<Route, IRouteHandler> pair
                in routes)
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
        private readonly IControllerRegistry controllerRegistry;
    }
}