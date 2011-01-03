using System.Collections.Generic;

namespace Detergent.Routing
{
    public class RouteMatchData
    {
        public IDictionary<string, string> Parameters
        {
            get { return parameters; }
        }

        public string GetParameter(string name)
        {
            return parameters[name];
        }

        private Dictionary<string, string> parameters = new Dictionary<string, string>();
    }
}