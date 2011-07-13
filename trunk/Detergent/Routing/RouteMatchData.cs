using System;
using System.Collections.Generic;

namespace Detergent.Routing
{
    public class RouteMatchData
    {
        public void AddParameter(string name, string value)
        {
            parameters.Add(name, value);
        }

        public string GetParameter(string name)
        {
            return parameters[name];
        }

        public bool HasParameter(string name)
        {
            return parameters.ContainsKey(name);
        }

        private Dictionary<string, string> parameters = new Dictionary<string, string>();
    }
}