using System;
using System.Reflection;
using System.Text.RegularExpressions;
using log4net;
using Microsoft.Http;

namespace Detergent.Routing
{
    public class Route
    {
        public Route(string urlScheme)
        {
            SetUrlScheme(urlScheme);
        }

        public Route(HttpMethod httpMethod, string urlScheme)
        {
            this.httpMethod = httpMethod.ToString();
            SetUrlScheme(urlScheme);
        }

        public RouteMatchData Match(IHttpContext httpContext)
        {
            if (false == DoesHttpMethodMatch(httpContext))
                return null;

            Match match = urlScheme.Match(httpContext.RequestPath);
            if (false == match.Success)
                return null;

            if (log.IsDebugEnabled)
                log.DebugFormat(
                    "Matched request path '{1}' to URL scheme '{0}'",
                    urlScheme,
                    httpContext.RequestPath);

            RouteMatchData data = new RouteMatchData();

            for (int i = 0; i < match.Groups.Count; i++)
            {
                string groupName = urlScheme.GroupNameFromNumber(i);
                data.Parameters[groupName] = match.Groups[i].Value;
            }

            return data;
        }

        private bool DoesHttpMethodMatch(IHttpContext context)
        {
            if (httpMethod == null)
                return true;

            return httpMethod == context.RequestHttpMethod;
        }

        private void SetUrlScheme(string urlScheme)
        {
            if (urlScheme == null)
                throw new ArgumentNullException("urlScheme", "You have to set the URL scheme for a route");

            this.urlScheme = new Regex(
                urlScheme, 
                RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        }

        private string httpMethod;
        private Regex urlScheme;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}