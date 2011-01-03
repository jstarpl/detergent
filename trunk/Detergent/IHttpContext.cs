using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;

namespace Detergent
{
    /// <summary>
    /// Represents the HTTP context used for reading the HTTP request information
    /// and sending the HTTP response.
    /// </summary>
    public interface IHttpContext
    {
        /// <summary>
        /// Gets the directory path of the Web application on the local disk.
        /// </summary>
        /// <value>The directory path of the Web application on the local disk.</value>
        string ApplicationDirectoryPath { get; }
        string ApplicationRootUrl { get; }        
        NameValueCollection QueryString { get; }
        Encoding RequestContentEncoding { get; }
        long RequestContentLength { get; }
        string RequestContentType { get; }
        string RequestHttpMethod { get; }
        string RequestId { get; }
        Stream RequestInputStream { get; }
        string RequestPath { get; }
        string RequestRawUrl { get; }
        string RequestUrl { get; }
        Encoding ResponseContentEncoding { get; set; }
        string ResponseContentType { get; set; }
        Stream ResponseStream { get; }
        int StatusCode { get; set; }

        void AddResponseHeader(string name, string value);
        void EndResponse();
        object GetAppCacheValue(string key);
        object GetContextCacheValue(string key);
        void Redirect(string url); 
        void RemoveContextCacheValue(string key);
        void SetAppCacheValue(string key, object value);
        void SetContextCacheValue(string key, object value);
        void SetExpiresHeader(DateTime expiryTime);
        void SetLastModifiedHeader(DateTime lastModifiedTime);
        void SetMaxAgeHeader(TimeSpan maxAge, HttpCacheability cacheability);
        void SetRequestId(string requestId);
        void SetResponse(byte[] data, string contentType);
        void SetResponse(byte[] data, string contentType, Encoding encoding);
        void SetResponse(string response, string contentType, Encoding encoding);
        void RespondWithFile(string fileName, string contentType);
    }
}