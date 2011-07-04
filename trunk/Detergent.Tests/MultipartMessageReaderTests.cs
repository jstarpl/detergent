using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Detergent.Mime;
using log4net;
using MbUnit.Framework;

namespace Detergent.Tests
{
    public class MultipartMessageReaderTests
    {
        [Test]
        public void ParseSimpleMessage()
        {
            string contentType = "multipart/mixed; boundary=\"abcd\"";
            string content = "\r\n--abcd\r\nContent-Type: application/xml;charset=UTF-8\r\n\r\n<movie>...</movie>\r\n--abcd--";

            MultipartMessage message = StartReading(contentType, content);
            Assert.IsTrue(message.IsMultipart);
            Assert.AreEqual(MultipartMessage.MediaTypeMultipartMixed, message.MediaType);
            Assert.AreEqual("abcd", message.Boundary);

            MultipartMessagePart part = reader.ReadNextPart();
            Assert.IsTrue(part.HasHeader("content-type"));
            Assert.AreEqual("application/xml", part.GetHeader("content-type").FieldValue);
            Assert.AreEqual("<movie>...</movie>", Encoding.UTF8.GetString(part.Data));

            Assert.IsNull(reader.ReadNextPart());
        }

        [Test]
        public void ParseMessageWithBoundaryWithoutQuotes()
        {
            string contentType = "multipart/mixed; boundary=abcd";
            string content = "\r\n--abcd\r\nContent-Type: application/xml;charset=UTF-8\r\n\r\n<movie>...</movie>\r\n--abcd--";

            MultipartMessage message = StartReading(contentType, content);
            Assert.IsTrue(message.IsMultipart);
            Assert.AreEqual(MultipartMessage.MediaTypeMultipartMixed, message.MediaType);
            Assert.AreEqual("abcd", message.Boundary);

            MultipartMessagePart part = reader.ReadNextPart();
            Assert.IsTrue(part.HasHeader("content-type"));
            Assert.AreEqual("application/xml", part.GetHeader("content-type").FieldValue);
            Assert.AreEqual("<movie>...</movie>", Encoding.UTF8.GetString(part.Data));

            Assert.IsNull(reader.ReadNextPart());
        }

        [Test]
        public void ParseMessageWithTwoParts()
        {
            string contentType = "multipart/mixed; boundary=\"abcd\"";
            string content = "\r\n--abcd\r\nContent-Type: application/xml;charset=UTF-8\r\n\r\n<movie>...</movie>\r\n--abcd\r\n\r\nplain text\r\n--abcd--";

            MultipartMessage message = StartReading(contentType, content);
            Assert.IsTrue(message.IsMultipart);
            Assert.AreEqual(MultipartMessage.MediaTypeMultipartMixed, message.MediaType);
            Assert.AreEqual("abcd", message.Boundary);

            MultipartMessagePart part = reader.ReadNextPart();
            Assert.IsTrue(part.HasHeader("content-type"));
            Assert.AreEqual("application/xml", part.GetHeader("content-type").FieldValue);
            Assert.AreEqual("<movie>...</movie>", Encoding.UTF8.GetString(part.Data));

            Assert.IsNotNull(reader.ReadNextPart());
            Assert.IsNull(reader.ReadNextPart());
        }

        [Test]
        public void ParseMessageWithBinaryData()
        {
            string boundary = "-----Test";

            byte[] requestData;
            using (MemoryStream stream = new MemoryStream())
            {
                using (MultipartStreamWriter writer = new MultipartStreamWriter(
                    stream,
                    boundary,
                    Encoding.UTF8))
                {
                    writer.WritePart("text/plain", "my text", Encoding.UTF8);
                    writer.WritePart("application/xml", "<tag/>", Encoding.UTF8);

                    byte[] pdfData = File.ReadAllBytes(@"..\..\..\samples\pdf-test.pdf");

                    List<HeaderField> headers = new List<HeaderField>();
                    headers.Add(new HeaderField("Content-type", "application/pdf"));
                    headers.Add(new HeaderField("Content-id", "PDF"));
                    writer.WritePart(headers, pdfData);
                }

                requestData = stream.ToArray();
            }

            log.DebugFormat("Message body: {0}", Encoding.UTF8.GetString(requestData));

            string requestContentType = string.Format(
                CultureInfo.InvariantCulture,
                "multipart/mixed;boundary=\"{0}\"",
                boundary);

            MultipartMessage message = StartReading(requestContentType, requestData);
            Assert.IsTrue(message.IsMultipart);
            Assert.AreEqual(MultipartMessage.MediaTypeMultipartMixed, message.MediaType);

            MultipartMessagePart part;
            
            part = reader.ReadNextPart();
            part = reader.ReadNextPart();
            part = reader.ReadNextPart();
            
            Assert.AreEqual("application/pdf", part.GetHeader("content-type").FieldValue);
            File.WriteAllBytes("test.pdf", part.Data);

            Assert.IsNull(reader.ReadNextPart());
        }

        //private static MultipartMessage ParseMessage(string contentType, string content)
        //{
        //    MultipartMessageParser parser = new MultipartMessageParser();
        //    byte[] contentData = Encoding.UTF8.GetBytes(content);

        //    if (log.IsDebugEnabled)
        //        log.DebugFormat("content: {0}", contentData.ArrayToString());

        //    using (MemoryStream stream = new MemoryStream(contentData))
        //        return parser.ParseMessage(contentType, stream);
        //}

        //private static MultipartMessage ParseMessage(string contentType, byte[] data)
        //{
        //    MultipartMessageParser parser = new MultipartMessageParser();
        //    using (MemoryStream stream = new MemoryStream(data))
        //        return parser.ParseMessage(contentType, stream);
        //}

        private MultipartMessage StartReading(string contentType, string content)
        {
            byte[] contentData = Encoding.UTF8.GetBytes(content);
            stream = new MemoryStream(contentData);
            reader = new MultipartMessageReader(stream);
            return reader.ReadHeader(contentType);
        }

        private MultipartMessage StartReading(string contentType, byte[] data)
        {
            stream = new MemoryStream(data);
            reader = new MultipartMessageReader(stream);
            return reader.ReadHeader(contentType);
        }

        [TearDown]
        private void Teardown()
        {
            if (reader != null)
                reader.Dispose();
            if (stream != null)
                stream.Dispose();
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MultipartMessageReader reader;
        private MemoryStream stream;
    }
}