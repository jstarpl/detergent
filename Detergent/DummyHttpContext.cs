using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;

namespace Detergent
{
    public class DummyHttpContext : IHttpContext
    {
        public DummyHttpContext()
        {
        }

        public DummyHttpContext(
            string applicationPath,
            string requestPath) : this (applicationPath, requestPath, null)
        {
        }

        [SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings")]
        public DummyHttpContext(
            string applicationPath,
            string requestPath,
            string queryString)
        {
            RequestPath = requestPath;
            
            Uri fullPath = new Uri(new Uri(applicationPath), requestPath);
            UriBuilder uri = new UriBuilder();
            uri.Path = fullPath.ToString();
            if (false == String.IsNullOrEmpty(queryString))
                uri.Query = queryString;

            this.RequestUrl = uri.ToString();
        }

        public string ApplicationDirectoryPath
        {
            get
            {
                string applicationFullPath = Assembly.GetExecutingAssembly().Location;
                return Path.GetDirectoryName(Path.GetFullPath(applicationFullPath));
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value")]
        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public string ApplicationRootUrl
        {
            get { return "test"; }
            set { throw new NotImplementedException(); }
        }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public NameValueCollection QueryString { get; set; }

        public Encoding RequestContentEncoding
        {
            get { return requestContentEncoding; }
            set { requestContentEncoding = value; }
        }

        public long RequestContentLength { get; set; }
        public string RequestContentType { get; set; }
        public string RequestHttpMethod { get; set; }

        public string RequestId
        {
            get { return "you wanna ID?"; }
        }

        public Stream RequestInputStream
        {
            get
            {
                return new MemoryStream(requestContentEncoding.GetBytes(requestContent), false);
            }
        }

        public string RequestPath { get; set; }
        public string RequestRawUrl { get; set; }
        public string RequestUrl { get; set; }

        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public Encoding ResponseContentEncoding
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public string ResponseContentType
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public Stream ResponseStream
        {
            get { throw new NotImplementedException(); }
        }

        public int StatusCode { get; set; }

        public void AddResponseHeader(string name, string value)
        {
            throw new NotImplementedException();
        }

        public void EndResponse()
        {
            throw new NotImplementedException();
        }

        public object GetAppCacheValue(string key)
        {
            return appCache[key];
        }

        public object GetContextCacheValue(string key)
        {
            throw new NotImplementedException();
        }

        public void Redirect(string url)
        {
            throw new NotImplementedException();
        }

        public void RemoveContextCacheValue(string key)
        {
            throw new NotImplementedException();
        }

        public void RespondWithFile(string fileName, string contentType)
        {
            throw new NotImplementedException();
        }

        public void SetAppCacheValue(string key, object value)
        {
            appCache[key] = value;
        }

        public void SetExpiresHeader(DateTime expiryTime)
        {
            throw new NotImplementedException();
        }

        public void SetLastModifiedHeader(DateTime lastModifiedTime)
        {
            throw new NotImplementedException();
        }

        public void SetMaxAgeHeader(TimeSpan maxAge, HttpCacheability cacheability)
        {
            throw new NotImplementedException();
        }

        public void SetContextCacheValue(string key, object value)
        {
            throw new NotImplementedException();
        }

        public void SetRequestId(string requestId)
        {
            throw new NotImplementedException();
        }

        public void SetResponse(byte[] data, string contentType)
        {
            throw new NotImplementedException();
        }

        public void SetRequestContent (
            string requestContentType, 
            Encoding requestContentEncoding,
            string requestContent)
        {
            this.requestContent = requestContent;
            this.requestContentEncoding = requestContentEncoding;
            RequestContentType = requestContentType;
            RequestContentLength = requestContentEncoding.GetByteCount(requestContent);
        }

        public void SetResponse(byte[] data, string contentType, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public void SetResponse(string response, string contentType, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        private Dictionary<string, object> appCache = new Dictionary<string, object>();
        private string requestContent;
        private Encoding requestContentEncoding = Encoding.UTF8;
    }
}