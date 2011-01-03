using System;
using System.Reflection;
using Detergent.Routing;
using log4net;

namespace Detergent.Mvc
{
    public class ControllerRouteHandler<TController> : IRouteHandler
         where TController : class, IController
    {
        public ControllerRouteHandler(
            IControllerRegistry controllerRegistry,
            Func<TController, IHttpResponse> handlerFunc)
        {
            this.controllerRegistry = controllerRegistry;
            this.handlerFunc = handlerFunc;
        }

        public IHttpResponse Handle(IHttpContext context, RouteMatchData routeMatchData)
        {
            if (log.IsDebugEnabled)
                log.DebugFormat(
                    "Calling controller+method '{0}.{1}'", 
                    typeof(TController).FullName,
                    handlerFunc.Method.Name);

            TController controller = controllerRegistry.FetchController<TController>();
            try
            {
                controller.Initialize(context, routeMatchData);
                return handlerFunc(controller);
            }
            finally
            {
                if (controller != null)
                    controllerRegistry.ReleaseController(controller);
            }
        }

        private readonly IControllerRegistry controllerRegistry;
        private Func<TController, IHttpResponse> handlerFunc;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}