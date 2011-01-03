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
    public class MultipartMessageParserTests
    {
        [Test]
        public void ParseHeaderWithoutParameters()
        {
            HeaderField field = HeaderField.Parse("Content-type  : multipart/mixed");
            Assert.AreEqual("Content-type", field.FieldName);
            Assert.AreEqual(MultipartMessage.MediaTypeMultipartMixed, field.FieldValue);
            Assert.AreEqual(0, field.Parameters.Count);
            Assert.AreEqual("Content-type: multipart/mixed", field.ToString());
        }

        [Test]
        public void ParseHeaderWithParameters()
        {
            HeaderField field = HeaderField.Parse("Content-type: multipart/mixed; boundary=\"23232\"; test=2");
            Assert.AreEqual("Content-type", field.FieldName);
            Assert.AreEqual(MultipartMessage.MediaTypeMultipartMixed, field.FieldValue);
            Assert.AreEqual(2, field.Parameters.Count);
            Assert.AreEqual("\"23232\"", field.Parameters["boundary"]);
            Assert.AreEqual("2", field.Parameters["test"]);
            Assert.AreEqual("Content-type: multipart/mixed; boundary=\"23232\"; test=2", field.ToString());
        }

        [Test]
        public void ParseSimpleMessage()
        {
            string contentType = "multipart/mixed; boundary=\"abcd\"";
            string content = "\r\n--abcd\r\nContent-Type: application/xml;charset=UTF-8\r\n\r\n<movie>...</movie>\r\n--abcd--";

            MultipartMessage message = ParseMessage(contentType, content);
            Assert.IsTrue(message.IsMultipart);
            Assert.AreEqual(MultipartMessage.MediaTypeMultipartMixed, message.MediaType);
            Assert.AreEqual("abcd", message.Boundary);
            Assert.AreEqual(1, message.Parts.Count);

            MultipartMessagePart part = message.Parts[0];
            Assert.IsTrue(part.HasHeader("content-type"));
            Assert.AreEqual("application/xml", part.GetHeader("content-type").FieldValue);
            Assert.AreEqual("<movie>...</movie>", Encoding.UTF8.GetString(part.Data));
        }

        [Test]
        public void ParseMessageWithBoundaryWithoutQuotes()
        {
            string contentType = "multipart/mixed; boundary=abcd";
            string content = "\r\n--abcd\r\nContent-Type: application/xml;charset=UTF-8\r\n\r\n<movie>...</movie>\r\n--abcd--";

            MultipartMessage message = ParseMessage(contentType, content);
            Assert.IsTrue(message.IsMultipart);
            Assert.AreEqual(MultipartMessage.MediaTypeMultipartMixed, message.MediaType);
            Assert.AreEqual("abcd", message.Boundary);
            Assert.AreEqual(1, message.Parts.Count);

            MultipartMessagePart part = message.Parts[0];
            Assert.IsTrue(part.HasHeader("content-type"));
            Assert.AreEqual("application/xml", part.GetHeader("content-type").FieldValue);
            Assert.AreEqual("<movie>...</movie>", Encoding.UTF8.GetString(part.Data));
        }

        [Test]
        public void ParseMessageWithTwoParts()
        {
            string contentType = "multipart/mixed; boundary=\"abcd\"";
            string content = "\r\n--abcd\r\nContent-Type: application/xml;charset=UTF-8\r\n\r\n<movie>...</movie>\r\n--abcd\r\n\r\nplain text\r\n--abcd--";

            MultipartMessage message = ParseMessage(contentType, content);
            Assert.IsTrue(message.IsMultipart);
            Assert.AreEqual(MultipartMessage.MediaTypeMultipartMixed, message.MediaType);
            Assert.AreEqual("abcd", message.Boundary);
            Assert.AreEqual(2, message.Parts.Count);

            MultipartMessagePart part = message.Parts[0];
            Assert.IsTrue(part.HasHeader("content-type"));
            Assert.AreEqual("application/xml", part.GetHeader("content-type").FieldValue);
            Assert.AreEqual("<movie>...</movie>", Encoding.UTF8.GetString(part.Data));
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

            MultipartMessage message = ParseMessage(requestContentType, requestData);
            Assert.IsTrue(message.IsMultipart);
            Assert.AreEqual(MultipartMessage.MediaTypeMultipartMixed, message.MediaType);
            Assert.AreEqual(3, message.Parts.Count);
            Assert.AreEqual("application/pdf", message.Parts[2].GetHeader("content-type").FieldValue);
            File.WriteAllBytes("test.pdf", message.Parts[2].Data);
        }

        private static MultipartMessage ParseMessage(string contentType, string content)
        {
            MultipartMessageParser parser = new MultipartMessageParser();
            byte[] contentData = Encoding.UTF8.GetBytes(content);

            if (log.IsDebugEnabled)
                log.DebugFormat("content: {0}", contentData.ArrayToString());

            using (MemoryStream stream = new MemoryStream(contentData))
                return parser.ParseMessage(contentType, stream);
        }

        private static MultipartMessage ParseMessage(string contentType, byte[] data)
        {
            MultipartMessageParser parser = new MultipartMessageParser();
            using (MemoryStream stream = new MemoryStream(data))
                return parser.ParseMessage(contentType, stream);
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}