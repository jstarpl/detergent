using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Detergent.Mime
{
    public class MultipartMessagePart
    {
        public IList<HeaderField> Headers
        {
            get { return headers; }
        }

        public void AddHeader(HeaderField header)
        {
            headers.Add(header);
        }

        public HeaderField GetHeader (string headerName)
        {
            HeaderField header = headers.Find(x => x.IsName(headerName));
            if (header == null)
            {
                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Header '{0}' not found.",
                    headerName);
                throw new KeyNotFoundException(message);
            }

            return header;
        }

        public bool HasHeader(string headerName)
        {
            return null != headers.Find(x => x.IsName(headerName));
        }

        public void RemoveHeader (string headerName)
        {
            headers.RemoveAll(x => x.IsName(headerName));
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        }

        private List<HeaderField> headers = new List<HeaderField>();
        private byte[] data;
    }
}