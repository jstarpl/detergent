using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using log4net;

namespace Detergent.Soap
{
    public class SoapEnvelope : ISoapEnvelope
    {
        public SoapEnvelope()
        {
        }

        public SoapEnvelope(string prefix, string namespaceUri)
        {
            this.prefix = prefix;
            this.namespaceUri = namespaceUri;
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string")]
        public SoapEnvelope Fault(SoapFaultCode faultCode, string faultString)
        {
            fault = new SoapFault(faultCode, faultString);
            return this;
        }

        public bool HasFault
        {
            get
            {
                return fault != null;
            }
        }

        public SoapEnvelope SetResponse(ISoapEnvelopeResponse response)
        {
            this.response = response;
            return this;
        }

        public void Send(IHttpContext context)
        {
            context.ResponseContentEncoding = Encoding.UTF8;
            context.ResponseContentType = SoapConstants.XmlContentType;
            SendEnvelope(context.ResponseStream);
        }

        public void SendEnvelope(HttpResponse httpResponse)
        {
            httpResponse.ContentEncoding = Encoding.UTF8;
            httpResponse.ContentType = SoapConstants.XmlContentType;

            SendEnvelope(httpResponse.OutputStream);
        }

        public void SendEnvelope(Stream outputStream)
        {
            using (StreamWriter writer = new StreamWriter(outputStream))
                SendEnvelope(writer);
        }

        public void SendEnvelope(TextWriter writer)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = false;
            settings.NewLineOnAttributes = false;
            settings.OmitXmlDeclaration = false;

            using (XmlWriter xmlWriter = XmlWriter.Create(writer, settings))
                WriteEnvelope(xmlWriter);
        }

        public string EnvelopeToString()
        {
            StringBuilder s = new StringBuilder();
            using (StringWriter writer = new StringWriter(s, CultureInfo.InvariantCulture))
                SendEnvelope(writer);

            return s.ToString();
        }

        private void WriteEnvelope(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("soapenv", "Envelope", SoapConstants.SoapEnvelopeNS);

            if (namespaceUri != null)
                xmlWriter.WriteAttributeString("xmlns", prefix, null, namespaceUri);

            xmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/1999/XMLSchema-instance");
            WriteEnvelopeHead(xmlWriter);
            WriteEnvelopeBody(xmlWriter);
            xmlWriter.WriteEndElement();
        }

        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "xmlWriter")]
        private void WriteEnvelopeHead(XmlWriter xmlWriter)
        {
        }

        private void WriteEnvelopeBody(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("soapenv", "Body", SoapConstants.SoapEnvelopeNS);
            WriteEnvelopeBodyFault(xmlWriter);
            WriteEnvelopeBodyResponse(xmlWriter);
            xmlWriter.WriteEndElement();
        }

        private void WriteEnvelopeBodyFault(XmlWriter xmlWriter)
        {
            if (fault != null)
            {
                xmlWriter.WriteStartElement("soapenv", "Fault", SoapConstants.SoapEnvelopeNS);

                xmlWriter.WriteStartElement("soapenv", "faultcode", SoapConstants.SoapEnvelopeNS);
                xmlWriter.WriteQualifiedName(fault.FaultCode.ToString(), SoapConstants.SoapEnvelopeNS);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteStartElement("soapenv", "faultstring", SoapConstants.SoapEnvelopeNS);
                xmlWriter.WriteString(fault.FaultString);
                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndElement();
            }
        }

        private void WriteEnvelopeBodyResponse(XmlWriter xmlWriter)
        {
            if (response != null)
            {
                string responseString = response.ToString();
                if (log.IsDebugEnabled)
                    log.DebugFormat("Response: {0}", responseString);
                xmlWriter.WriteRaw(responseString);
                //xmlWriter.WriteStartElement(response.ResponseName, response.ResponseNamespaceUri);

                //xmlWriter.WriteStartElement(response.Values[0].Key);
                //xmlWriter.WriteValue(response.Values[0].Value);
                //xmlWriter.WriteEndElement();

                //xmlWriter.WriteEndElement();
            }
        }

        private SoapFault fault;
        private static readonly ILog log = LogManager.GetLogger(typeof(SoapEnvelope));
        private ISoapEnvelopeResponse response;
        private readonly string prefix;
        private readonly string namespaceUri;
    }
}