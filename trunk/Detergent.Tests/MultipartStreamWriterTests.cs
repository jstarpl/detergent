using System.IO;
using System.Text;
using Detergent.Mime;
using MbUnit.Framework;

namespace Detergent.Tests
{
    public class MultipartStreamWriterTests
    {
        [Test]
        public void WriteManuallyPartByPart()
        {
            string boundary = "-----Test";

            using (MemoryStream stream = new MemoryStream())
            {
                using (MultipartStreamWriter writer = new MultipartStreamWriter(
                    stream,
                    boundary,
                    Encoding.UTF8))
                {
                    writer.WritePart("text/plain", "my text", Encoding.UTF8);
                    writer.WritePart("application/xml", "<tag/>", Encoding.UTF8);
                    writer.WriteCloseDelimiter();
                }

                byte[] bytes = stream.ToArray();
                string result = Encoding.UTF8.GetString(bytes);
                Assert.AreEqual("\r\n-------Test\r\nContent-Type: text/plain\r\n\r\nmy text\r\n-------Test\r\nContent-Type: application/xml\r\n\r\n<tag/>\r\n-------Test--", result);
            }
        }

        [Test]
        public void WriteMessageAsAWhole()
        {
            string boundary = "-----Test";

            MultipartMessage message = new MultipartMessage(HeaderField.Parse("Content-type: multipart/mixed; boundary=-----Test"));

            MultipartMessagePart part;

            part = new MultipartMessagePart();
            part.AddHeader(new HeaderField("Content-Type", "text/plain"));
            part.Data = Encoding.UTF8.GetBytes("my text");
            message.AddPart(part);

            part = new MultipartMessagePart();
            part.AddHeader(new HeaderField("Content-Type", "application/xml"));
            part.Data = Encoding.UTF8.GetBytes("<tag/>");
            message.AddPart(part);

            using (MemoryStream stream = new MemoryStream())
            {
                long writtenBytes;
                using (MultipartStreamWriter writer = new MultipartStreamWriter(
                    stream,
                    boundary,
                    Encoding.UTF8))
                {
                    writer.WriteWholeMessage(message);
                    writtenBytes = writer.BytesWritten;
                }

                byte[] bytes = stream.ToArray();
                Assert.AreEqual(bytes.Length, writtenBytes);
                string result = Encoding.UTF8.GetString(bytes);
                Assert.AreEqual("\r\n-------Test\r\nContent-Type: text/plain\r\n\r\nmy text\r\n-------Test\r\nContent-Type: application/xml\r\n\r\n<tag/>\r\n-------Test--", result);
            }
        }
    }
}