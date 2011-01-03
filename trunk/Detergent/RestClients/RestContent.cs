using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Xml.Serialization;

namespace Detergent.RestClients
{
    public class RestContent
    {
        public RestContent(string content, Uri requestUrl)
        {
            this.contentString = content;
            this.requestUrl = requestUrl;
        }

        public string ContentString
        {
            get
            {
                return contentString;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public T Deserialize<T>()
        {
            try
            {
                XmlSerializer serializer = xmlSerializerFactory.GetSerializerFor<T>();
                using (TextReader textReader = new StringReader(ContentString))
                    return (T)serializer.Deserialize(textReader);
            }
            catch (Exception ex)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Could not deserialize data of type '{0}'",
                    typeof(T).Name);

                throw new RestDataDeserializationException(requestUrl, message, ex);
            }
        }

        private string contentString;
        private readonly Uri requestUrl;
        private static XmlSerializerFactory xmlSerializerFactory = new XmlSerializerFactory();
    }
}