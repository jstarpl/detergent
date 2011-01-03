using System;
using System.Reflection;
using Detergent.Routing;
using log4net;

namespace Detergent.Rest
{
    public class ServiceRouteHandler<TService> : IRouteHandler
        where TService : IRestService
    {
        public ServiceRouteHandler(
            IRestServicesRegistry servicesRegistry,
            Func<TService, IHttpResponse> handlerFunc)
        {
            this.servicesRegistry = servicesRegistry;
            this.handlerFunc = handlerFunc;
        }

        public IHttpResponse Handle(IHttpContext context, RouteMatchData routeMatchData)
        {
            string serviceName = typeof(TService).FullName;

            if (log.IsDebugEnabled)
                log.DebugFormat(
                    "Calling service+method '{0}.{1}'", 
                    serviceName,
                    handlerFunc.Method.Name);

            TService service;

            try
            {
                service = servicesRegistry.FetchService<TService>();
            }
            catch (Exception ex)
            {
                log.FatalFormat("Cannot instantiate REST service '{0}'. Reason: {1}", serviceName, ex);
                throw;
            }
            
            try
            {
                service.Initialize(context, routeMatchData);
                return handlerFunc(service);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("REST service '{0}' failed. Reason: {1}", serviceName, ex);
                throw;
            }
            finally
            {
                if (service != null)
                    servicesRegistry.ReleaseService(service);
            }
        }

        private readonly IRestServicesRegistry servicesRegistry;
        private Func<TService, IHttpResponse> handlerFunc;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}