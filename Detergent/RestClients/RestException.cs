using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace Detergent.RestClients
{
    [SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly")]
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors")]
    [Serializable]
    public class RestException : Exception
    {
        public RestException(
            Uri requestUrl,
            HttpStatusCode httpStatusCode, 
            string message,
            string requestId,
            Exception inner)
            : base(message, inner)
        {
            this.requestUrl = requestUrl;
            this.httpStatusCode = httpStatusCode;
            this.requestId = requestId;
        }

        public RestException(
            Uri requestUrl,
            HttpStatusCode httpStatusCode,
            string message,
            string requestId)
            : base(message)
        {
            this.requestUrl = requestUrl;
            this.httpStatusCode = httpStatusCode;
            this.requestId = requestId;
        }

        public RestException(Uri requestUrl, string message, Exception inner)
            : base(message, inner)
        {
            this.requestUrl = requestUrl;
        }

        public HttpStatusCode HttpStatusCode
        {
            get { return httpStatusCode; }
        }

        public string RequestId
        {
            get { return requestId; }
        }

        public static RestException CreateFromHttpResponse(
            Uri requestUrl,
            HttpWebResponse response)
        {
            string errorDescription = FetchHttpHeaderIfExists(
                response, 
                HttpHeaderErrorDescription,
                RestClient.ReadResponseAsString(response));

            if (String.IsNullOrEmpty(errorDescription))
                errorDescription = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0} {1}",
                    response.StatusCode,
                    response.StatusDescription);

            string requestId = FetchHttpHeaderIfExists(
                response,
                HttpHeaderRequestId,
                null);

            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return new RestNotFoundException(
                        requestUrl,
                        response.StatusCode, 
                        errorDescription,
                        requestId);
                case HttpStatusCode.InternalServerError:
                    return new RestInternalServerException(
                        requestUrl,
                        response.StatusCode, 
                        errorDescription,
                        requestId);
                case HttpStatusCode.ServiceUnavailable:
                    return new RestServiceUnavailableException(
                        requestUrl,
                        response.StatusCode,
                        errorDescription,
                        requestId);
                case HttpStatusCode.Forbidden:
                    return new RestForbiddenOperationException(
                        requestUrl,
                        response.StatusCode,
                        errorDescription,
                        requestId);
                default:
                    return new RestException(
                        requestUrl,
                        response.StatusCode, 
                        errorDescription,
                        requestId);
            }
        }

        public override string ToString()
        {
            return ExceptionToString(
                description => description.AppendFormat(
                                   ", RequestUrl='{0}', HttpStatusCode={1}, RequestId='{2}'", 
                                   requestUrl,
                                   httpStatusCode, 
                                   RequestId));
        }

        protected RestException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public string ExceptionToString(
            Action<StringBuilder> customFieldsFormatterAction)
        {
            StringBuilder description = new StringBuilder();
            description.AppendFormat("{0}: {1}", this.GetType().Name, this.Message);

            if (customFieldsFormatterAction != null)
                customFieldsFormatterAction(description);

            if (this.InnerException != null)
            {
                description.AppendFormat(" ---> {0}", this.InnerException);
                description.AppendFormat(
                    "{0}   --- End of inner exception stack trace ---{0}",
                    Environment.NewLine);
            }

            description.Append(this.StackTrace);

            return description.ToString();
        }

        private static string FetchHttpHeaderIfExists(
            HttpWebResponse response,
            string httpHeaderId,
            string defaultValue)
        {
            string value = response.Headers.Get(httpHeaderId);
            return value ?? defaultValue;
        }

        private const string HttpHeaderErrorDescription = "ErrorDescription";
        private const string HttpHeaderRequestId = "RequestId";
        private readonly Uri requestUrl;
        private HttpStatusCode httpStatusCode;
        private readonly string requestId;
    }
}