using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;

namespace Detergent
{
    /// <summary>
    /// An implementation of the <see cref="IHttpContext"/> interface which
    /// wraps the <see cref="HttpContext"/> object.
    /// </summary>
    public class WebHttpContext : IHttpContext
    {
        public WebHttpContext(HttpContext httpContext)
        {
            this.httpContext = httpContext;
        }

        public void AddResponseHeader(string name, string value)
        {
            httpContext.Response.AddHeader(name, value);
        }

        public void EndResponse()
        {
            httpContext.Response.End();
        }

        public object GetAppCacheValue(string key)
        {
            return httpContext.Application[key];
        }

        public object GetContextCacheValue(string key)
        {
            return httpContext.Items[key];
        }

        public void RemoveContextCacheValue(string key)
        {
            httpContext.Items.Remove(key);
        }

        public string ApplicationDirectoryPath
        {
            get
            {
                string applicationFullPath = Assembly.GetExecutingAssembly().Location;
                return Path.GetDirectoryName(Path.GetFullPath(applicationFullPath));
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public string ApplicationRootUrl
        {
            get { throw new NotImplementedException(); }
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
            get { return httpContext.Request.ContentLength; }
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
            get { return httpContext.Request.InputStream; }
        }

        public string RequestPath
        {
            get { return httpContext.Request.Path; }
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

        public void Redirect(string url)
        {
            httpContext.Response.Redirect(url);
        }

        public void SetAppCacheValue(string key, object value)
        {
            httpContext.Application[key] = value;
        }

        public void SetContextCacheValue(string key, object value)
        {
            httpContext.Items[key] = value;
        }

        public void SetExpiresHeader(DateTime expiryTime)
        {
            httpContext.Response.Cache.SetExpires(expiryTime);
        }

        public void SetLastModifiedHeader(DateTime lastModifiedTime)
        {
            httpContext.Response.Cache.SetLastModified(lastModifiedTime);
        }

        public void SetMaxAgeHeader(TimeSpan maxAge, HttpCacheability cacheability)
        {
            httpContext.Response.Cache.SetMaxAge(maxAge);
            httpContext.Response.Cache.SetCacheability(cacheability);
        }

        public void SetRequestId(string requestId)
        {
            this.requestId = requestId;
        }

        public void SetResponse(byte[] data, string contentType)
        {
            httpContext.Response.ContentType = contentType;
            httpContext.Response.BinaryWrite(data);
            httpContext.Response.End();
        }

        public void SetResponse(byte[] data, string contentType, Encoding encoding)
        {
            httpContext.Response.ContentEncoding = encoding;
            httpContext.Response.ContentType = contentType;
            httpContext.Response.BinaryWrite(data);
            httpContext.Response.End();
        }

        public void SetResponse(string response, string contentType, Encoding encoding)
        {
            httpContext.Response.ContentEncoding = encoding;
            httpContext.Response.ContentType = contentType;
            httpContext.Response.BinaryWrite(encoding.GetBytes(response));
            httpContext.Response.End();
        }

        public void RespondWithFile(string fileName, string contentType)
        {
            httpContext.Response.ContentType = contentType;
            httpContext.Response.TransmitFile(fileName);
            httpContext.Response.End();
        }

        private readonly HttpContext httpContext;
        private string requestId;
    }
}