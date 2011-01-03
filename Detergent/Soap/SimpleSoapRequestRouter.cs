using System;
using System.Collections.Generic;
using System.Xml;

namespace Detergent.Soap
{
    public class SimpleSoapRequestRouter : ISoapRequestRouter
    {
        public void AddRoute(
            string methodName, 
            string methodNamespace,
            Func<IHttpContext, XmlNode, ISoapEnvelope> methodAction)
        {
            routes.Add(new KeyValuePair<string, string>(methodName, methodNamespace), methodAction);
        }

        public ISoapEnvelope RouteSoapRequest(
            IHttpContext httpContext, 
            XmlNode requestNode)
        {
            KeyValuePair<string, string> key = new KeyValuePair<string, string>(
                requestNode.LocalName, requestNode.NamespaceURI);

            if (routes.ContainsKey(key))
                return routes[key](httpContext, requestNode);

            return null;
        }

        private Dictionary<KeyValuePair<string, string>, Func<IHttpContext, XmlNode, ISoapEnvelope>> routes
            = new Dictionary<KeyValuePair<string, string>, Func<IHttpContext, XmlNode, ISoapEnvelope>>();
    }
}