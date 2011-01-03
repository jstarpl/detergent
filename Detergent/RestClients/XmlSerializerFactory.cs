using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using log4net;

namespace Detergent.RestClients
{
    [ComVisible(false)]
    public class XmlSerializerFactory
    {
        public XmlSerializer GetSerializerFor<T>()
        {
            lock (this)
            {
                Type typeOfT = typeof(T);
                if (false == serializers.ContainsKey(typeOfT))
                {
                    XmlSerializer newSerializer = new XmlSerializer(typeOfT);
                    serializers.Add(typeOfT, newSerializer);

                    if (log.IsDebugEnabled)
                        log.DebugFormat("Created XmlSerializer for {0}", typeOfT.FullName);
                }

                return serializers[typeOfT];
            }
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(XmlSerializerFactory));
        private Dictionary<Type, XmlSerializer> serializers = new Dictionary<Type, XmlSerializer>();
    }
}