using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Http;
using Wintellect.PowerCollections;

namespace Detergent.RestClients
{
    public class RestRequest
    {
        public RestRequest(HttpMethod operation, string urlFormat, params object[] urlFormatArgs)
        {
            this.operation = operation;
            this.urlFormat = urlFormat;
            this.urlFormatArgs = urlFormatArgs;
        }

        public bool HasQueryParameters
        {
            get
            {
                return queryParameters != null && queryParameters.Count > 0;
            }
        }

        public HttpMethod Operation
        {
            get { return operation; }
        }

        public string UrlFormat
        {
            get { return urlFormat; }
        }

        public bool UrlShouldBeFormatted
        {
            get { return urlFormatArgs != null && urlFormatArgs.Length > 0; }
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public object[] UrlFormatArgs
        {
            get { return urlFormatArgs; }
        }

        public MultiDictionary<string, object> QueryParameters
        {
            get { return queryParameters; }
        }

        public IList<object> ListQueryParameterValues(string parameterName)
        {
            return new List<object>(queryParameters[parameterName]);
        }

        public void SetQueryParameterValue(string parameterName, object parameterValue)
        {
            queryParameters.Remove(parameterName);
            queryParameters.Add(parameterName, parameterValue);
        }

        private HttpMethod operation;
        private readonly string urlFormat;
        private readonly object[] urlFormatArgs;
        private MultiDictionary<string, object> queryParameters = new MultiDictionary<string, object>(true);
    }
}