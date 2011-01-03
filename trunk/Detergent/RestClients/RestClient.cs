using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using log4net;
using Microsoft.Http;

namespace Detergent.RestClients
{
    public class RestClient : IRestClient
    {
        public RestClient(RestClientConfiguration restClientConfiguration)
        {
            this.restClientConfiguration = restClientConfiguration;
        }

        public RestContent Call(RestRequest request)
        {
            string url = ConstructUri(request);
            return SendHttpRequest(url, request);
        }

        public RestContent Send<T>(RestRequest request, T objectToSend) where T : class 
        {
            string url = ConstructUri(request);
            return SendHttpRequest(url, request, objectToSend);
        }

        public string ConstructUri(RestRequest request)
        {
            string queryString = null;

            if (request.HasQueryParameters)
                queryString = BuildQueryString(request);

            string newFormat = request.UrlFormat;
            string expandedUri;
            
            if (request.UrlShouldBeFormatted)
                expandedUri = String.Format(CultureInfo.InvariantCulture, newFormat, request.UrlFormatArgs);
            else
                expandedUri = newFormat;

            string uriPath = CombineUrl(restClientConfiguration.WebServiceBaseUrl, expandedUri);
            
            return UrlEncode(uriPath + queryString);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">If <code>false</code>, cleans up native resources. 
        /// If <code>true</code> cleans up both managed and native resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (false == disposed)
            {
                // clean native resources         

                if (disposing)
                {
                }

                disposed = true;
            }
        }

        private static string CombineUrl(string uri1, string uri2)
        {
            uri1 = uri1.TrimEnd('/');
            uri2 = uri2.TrimStart('/');
            return string.Format(CultureInfo.InvariantCulture, "{0}/{1}", uri1, uri2);
        }

        private static string BuildQueryString(RestRequest request)
        {
            bool addQuestionMark = request.UrlFormat.Contains("?");

            StringBuilder queryString = new StringBuilder();
            if (addQuestionMark)
                queryString.Append('&');
            else
                queryString.Append('?');

            string separator = string.Empty;

            foreach (KeyValuePair<string, ICollection<object>> pairs in request.QueryParameters)
            {
                foreach (object valueObject in pairs.Value)
                {
                    string value = Convert.ToString(valueObject, CultureInfo.InvariantCulture);

                    if (pairs.Key != "type")
                    {
                        queryString.Append(separator);
                        queryString.Append(HttpUtility.UrlEncode(pairs.Key));
                        queryString.Append('=');
                        queryString.Append(HttpUtility.UrlEncode(value));

                        separator = "&";
                    }
                }
            }

            return queryString.ToString();
        }

        private static void RaiseOERestExceptionIfNeeded(Uri requestUrl, HttpWebResponse response)
        {
            if (response == null)
                throw new ArgumentNullException("response");

            if (response.StatusCode != HttpStatusCode.OK)
            {
                RestException restEx = RestException.CreateFromHttpResponse(requestUrl, response);
                throw restEx;
            }
        }

        private void SetupWebRequest(HttpWebRequest request)
        {
            request.Timeout = (int)restClientConfiguration.RequestTimeout.TotalMilliseconds;

            if (restClientConfiguration.UseCredentials)
            {
                request.PreAuthenticate = true;
                
                request.Credentials = restClientConfiguration.Credentials;
            }

            if (restClientConfiguration.UseProxy)
            {
                WebProxy proxy = new WebProxy(restClientConfiguration.ProxyAddress);
                proxy.Credentials = restClientConfiguration.ProxyCredentials;

                request.Proxy = proxy;
            }
        }

        private RestContent SendHttpRequest(
            string url, 
            RestRequest request)
        {
            return SendHttpRequest<object>(url, request, null);    
        }

        private RestContent SendHttpRequest<T>(
            string url,
            RestRequest request,
            T objectToSend) 
            where T : class
        {
            if (log.IsDebugEnabled)
                log.DebugFormat(
                    "Calling OE REST (operation='{0}', url='{1}')", 
                    request.Operation,
                    url);

            Uri requestUrl = new Uri(url);

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Method = request.Operation.ToString();
            webRequest.ContentType = "text/xml";

            SetupWebRequest(webRequest);

            try
            {
                if (objectToSend != null)
                {
                    using (Stream requestStream = webRequest.GetRequestStream())
                    {
                        using (HttpContent httpContent = 
                            HttpContentExtensions.CreateXmlSerializable(objectToSend))
                        {
                            if (log.IsDebugEnabled)
                            {
                                httpContent.LoadIntoBuffer();
                                log.DebugFormat("Sending XML: {0}", httpContent.ReadAsString());
                            }

                            httpContent.WriteTo(requestStream);
                        }
                    }
                }

                using (HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse())
                {
                    RaiseOERestExceptionIfNeeded(requestUrl, response);

                    RestContent oecontent;

                    oecontent = new RestContent(
                        ReadResponseAsString(response),
                        requestUrl);

                    if (log.IsDebugEnabled)
                        log.DebugFormat("Response: '{0}'", oecontent.ContentString);

                    return oecontent;
                }
            }
            catch (WebException ex)
            {
                if (ex.Response == null)
                {
                    if (log.IsDebugEnabled)
                        log.Debug("ex.Response == null");

                    string message = string.Format(
                        CultureInfo.InvariantCulture,
                        "The call to '{0}' failed",
                        url);

                    throw new RestException(requestUrl, message, ex);
                }

                RaiseOERestExceptionIfNeeded(requestUrl, (HttpWebResponse)ex.Response);
                throw;
            }
            catch (Exception ex)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Exception while calling '{0}'",
                    url);

                log.Warn(message, ex);
                throw;
            }
        }

        public static string ReadResponseAsString (WebResponse response)
        {
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        private static string UrlEncode(string uri)
        {
            return HttpUtility.UrlPathEncode(uri);
        }

        private bool disposed;
        private static readonly ILog log = LogManager.GetLogger(typeof(RestClient));
        private readonly RestClientConfiguration restClientConfiguration;
    }
}