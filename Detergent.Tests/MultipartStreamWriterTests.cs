using System.IO;
using System.Text;
using Detergent.Mime;
using MbUnit.Framework;

namespace Detergent.Tests
{
    public class MultipartStreamWriterTests
    {
        [Test]
        public void Test()
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
                }

                byte[] bytes = stream.ToArray();
                string result = Encoding.UTF8.GetString(bytes);
                Assert.AreEqual("\r\n-------Test\r\nContent-type: text/plain\r\n\r\nmy text\r\n-------Test\r\nContent-type: application/xml\r\n\r\n<tag/>\r\n-------Test--", result);
            }
        }
    }
}