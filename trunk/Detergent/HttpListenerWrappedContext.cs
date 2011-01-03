using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;

namespace Detergent
{
    /// <summary>
    /// An implementation of the <see cref="IHttpContext"/> interface which
    /// wraps the <see cref="HttpListenerContext"/> object.
    /// </summary>
    public class HttpListenerWrappedContext : IHttpContext
    {
        public HttpListenerWrappedContext(
            HttpListenerContext httpContext,
            Stream requestStream,
            string applicationRootUrl,
            string applicationPath)
        {
            this.httpContext = httpContext;
            this.requestStream = requestStream;
            this.applicationRootUrl = applicationRootUrl;
            this.applicationPath = applicationPath;
        }

        public string ApplicationDirectoryPath
        {
            get
            {
                string applicationFullPath = Assembly.GetExecutingAssembly().Location;
                return Path.GetDirectoryName(Path.GetFullPath(applicationFullPath));
            }
        }

        public string ApplicationRootUrl
        {
            get { return applicationRootUrl; }
        }

        public NameValueCollection QueryString
        {
            get { return httpContext.Request.QueryString; }
        }

        public Encoding RequestContentEncoding
        {
            get { return httpContext.Request.ContentEncoding; }
        }

        public long RequestContentLength
        {
            get { return httpContext.Request.ContentLength64; }
        }

        public string RequestContentType
        {
            get { return httpContext.Request.ContentType; }
        }

        public string RequestHttpMethod
        {
            get { return httpContext.Request.HttpMethod; }
        }

        public string RequestId
        {
            get { return requestId; }
        }

        public Stream RequestInputStream
        {
            get { return requestStream; }
        }

        public string RequestPath
        {
            get
            {
                Uri uri = new Uri(RequestUrl);
                string localPath = uri.LocalPath;

                if (applicationPath.Length <= localPath.Length)
                    return localPath.Substring(applicationPath.Length);

                return string.Empty;
            }
        }

        public string RequestRawUrl
        {
            get { return httpContext.Request.RawUrl; }
        }

        public string RequestUrl
        {
            get { return httpContext.Request.Url.ToString(); }
        }

        public Encoding ResponseContentEncoding
        {
            get { return httpContext.Response.ContentEncoding; }
            set { httpContext.Response.ContentEncoding = value; }
        }

        public string ResponseContentType
        {
            get { return httpContext.Response.ContentType; }
            set { httpContext.Response.ContentType = value; }
        }

        public Stream ResponseStream
        {
            get { return httpContext.Response.OutputStream; }
        }

        public int StatusCode
        {
            get { return httpContext.Response.StatusCode; }
            set { httpContext.Response.StatusCode = value; }
        }

        public void AddResponseHeader(string name, string value)
        {
            httpContext.Response.AddHeader(name, value);
        }

        public void EndResponse()
        {
            httpContext.Response.Close();
        }

        public object GetAppCacheValue(string key)
        {
            lock (appCache)
                return appCache[key];
        }

        public object GetContextCacheValue(string key)
        {
            lock (contextCache)
            {
                if (contextCache.ContainsKey(key))
                    return contextCache[key];

                return null;
            }
        }

        public void Redirect(string url)
        {
            httpContext.Response.Redirect(url);
            httpContext.Response.Close();
        }

        public void RemoveContextCacheValue(string key)
        {
            lock (contextCache)
                contextCache.Remove(key);
        }

        public void SetAppCacheValue(string key, object value)
        {
            lock (appCache)
                appCache[key] = value;
        }

        public void SetExpiresHeader(DateTime expiryTime)
        {
            AddResponseHeader("Expires", expiryTime.ToUniversalTime().ToString("r", CultureInfo.InvariantCulture));
        }

        public void SetLastModifiedHeader(DateTime lastModifiedTime)
        {
            AddResponseHeader("Last-Modified", lastModifiedTime.ToUniversalTime().ToString("r", CultureInfo.InvariantCulture));
        }

        public void SetMaxAgeHeader(TimeSpan maxAge, HttpCacheability cacheability)
        {
            string maxAgeString = string.Format(
                CultureInfo.InvariantCulture,
                "max-age={0}",
                (int)maxAge.TotalSeconds);
            AddResponseHeader("Cache-Control", maxAgeString);
        }

        public void SetContextCacheValue(string key, object value)
        {
            lock (contextCache)
                contextCache[key] = value;
        }

        public void SetRequestId(string requestId)
        {
            this.requestId = requestId;
        }

        public void SetResponse(byte[] data, string contentType)
        {
            httpContext.Response.ContentType = contentType;
            httpContext.Response.Close(data, true);
        }

        public void SetResponse(byte[] data, string contentType, Encoding encoding)
        {
            httpContext.Response.ContentType = contentType;
            httpContext.Response.ContentEncoding = encoding;
            httpContext.Response.Close(data, true);
        }

        public void SetResponse(string response, string contentType, Encoding encoding)
        {
            httpContext.Response.ContentType = contentType;
            httpContext.Response.ContentEncoding = encoding;
            httpContext.Response.Close(encoding.GetBytes(response), true);
        }

        public void RespondWithFile(string fileName, string contentType)
        {
            httpContext.Response.ContentType = contentType;
            byte[] data = File.ReadAllBytes(fileName);
            httpContext.Response.Close(data, true);
        }

        private Dictionary<string, object> appCache = new Dictionary<string, object>();
        [ThreadStatic]
        private static Dictionary<string, object> contextCache = new Dictionary<string, object>();
        private readonly string applicationPath;
        private readonly HttpListenerContext httpContext;
        private readonly string applicationRootUrl;
        private readonly Stream requestStream;
        private string requestId;
    }
}