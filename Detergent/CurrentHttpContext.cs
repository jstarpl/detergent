using System;

namespace Detergent
{
    public static class CurrentHttpContext
    {
        public static IHttpContext Current
        {
            get { return current; }
            set { current = value; }
        }

        [ThreadStatic]
        private static IHttpContext current;
    }
}