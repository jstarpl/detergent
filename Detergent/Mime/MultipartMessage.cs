using System.Collections.Generic;
using System.Text;

namespace Detergent.Mime
{
    public class MultipartMessage
    {
        public const string MediaTypeMultipartMixed = "multipart/mixed";
        public const string ParameterBoundary = "boundary";

        public MultipartMessage(HeaderField contentType)
        {
            this.contentType = contentType;
        }

        public Encoding BaseEncoding
        {
            get { return baseEncoding; }
        }

        public string Boundary
        {
            get
            {
                string boundary = contentType.Parameters[ParameterBoundary];
                if (boundary.StartsWith("\"") && boundary.EndsWith("\""))
                    return boundary.Substring(1, boundary.Length-2);

                return boundary;
            }
        }

        public bool IsMultipart
        {
            get { return MediaType.StartsWith("multipart"); }
        }

        public string MediaType
        {
            get { return contentType.FieldValue; }
        }

        public IList<MultipartMessagePart> Parts
        {
            get { return parts; }
        }

        public void AddPart(MultipartMessagePart part)
        {
            parts.Add(part);
        }

        private readonly HeaderField contentType;
        private List<MultipartMessagePart> parts = new List<MultipartMessagePart>();
        private Encoding baseEncoding = Encoding.UTF8;
    }
}