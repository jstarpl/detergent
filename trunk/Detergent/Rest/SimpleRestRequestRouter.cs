using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Detergent.Routing;
using Microsoft.Http;

namespace Detergent.Rest
{
    public class SimpleRestRequestRouter : IRestRequestRouter
    {
        public void AddRoute(
            HttpMethod httpMethod,
            string urlScheme,
            Func<IHttpContext, RouteMatchData, IHttpResponse> methodAction)
        {
            routes.Add(
                new Route(httpMethod, urlScheme),
                new StaticRouteHandler(methodAction));
        }
        
        public IHttpResponse RouteRestRequest(IHttpContext context)
        {
            foreach (KeyValuePair<Route, IRouteHandler> pair 
                in routes)
            {
                RouteMatchData matchData = pair.Key.Match(context);
                if (matchData == null)
                    continue;

                IHttpResponse response = pair.Value.Handle(context, matchData);
                return response;
            }

            throw new RestBadRequestException(new Uri(context.RequestUrl), null, null);
        }

        private Dictionary<Route, IRouteHandler> routes =
            new Dictionary<Route, IRouteHandler>();
    }
}