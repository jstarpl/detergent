using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Detergent.RestClients
{
    [ComVisible(false)]
    public class RestClientConfiguration
    {
        public ICredentials Credentials { get; set; }
        public string ProxyAddress { get; set; }
        public ICredentials ProxyCredentials { get; set; }
        public TimeSpan RequestTimeout
        {
            get { return requestTimeout; }
            set { requestTimeout = value; }
        }

        public bool UseCredentials { get; set; }
        public bool UseProxy { get; set; }
        public string WebServiceBaseUrl
        {
            get { return webServiceBaseUrl; }
            set { webServiceBaseUrl = value; }
        }

        public bool UseMockedClient { get; set; }

        public bool UseCleanUrl 
        {
            get { return useCleanUrl; }
            set { useCleanUrl = value; }
        }

        private bool useCleanUrl = false;
        private TimeSpan requestTimeout = TimeSpan.FromSeconds(60);
        private string webServiceBaseUrl = "http://localhost/eEnvoyer";
    }
}