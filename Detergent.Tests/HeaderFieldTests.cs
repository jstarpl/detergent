using Detergent.Mime;
using MbUnit.Framework;

namespace Detergent.Tests
{
    public class HeaderFieldTests
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
        public void ParseHeaderWithParameters2()
        {
            HeaderField field = HeaderField.Parse("Content-Disposition: attachment; filename=test.pdf");
            Assert.AreEqual("Content-Disposition", field.FieldName);
            Assert.AreEqual("attachment", field.FieldValue);
            Assert.AreEqual(1, field.Parameters.Count);
            Assert.AreEqual("test.pdf", field.Parameters["filename"]);
        }

        [Test]
        public void HeaderNamesAreCaseInsensitive()
        {
            HeaderField field = new HeaderField(HttpConstants.ContentType, "value");
            Assert.IsTrue(field.IsName("Content-type"));
        }
    }
}