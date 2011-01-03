using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Detergent.Mime
{
    public class MultipartMessagePart
    {
        public MultipartMessagePart()
        {
        }

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
            HeaderField header = headers.Find(x => 0 == string.Compare(headerName, x.FieldName, StringComparison.OrdinalIgnoreCase));            
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
            return null != headers.Find(x => 0 == string.Compare(headerName, x.FieldName, StringComparison.OrdinalIgnoreCase));
        }

        public void RemoveHeader (string headerName)
        {
            headers.RemoveAll(x => 0 == string.Compare(headerName, x.FieldName, StringComparison.OrdinalIgnoreCase));
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