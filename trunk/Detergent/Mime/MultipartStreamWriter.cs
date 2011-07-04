using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Detergent.Mime
{
    public class MultipartStreamWriter : IDisposable
    {
        public MultipartStreamWriter(Stream stream, string boundary, Encoding baseEncoding)
        {
            this.boundary = boundary;
            this.baseEncoding = baseEncoding;
            binaryWriter = new BinaryWriter(stream);
        }

        public void WriteWholeMessage(MultipartMessage message)
        {
            foreach (MultipartMessagePart part in message.Parts)
                WritePart(part.Headers, part.Data);
        }

        public void WritePart (string contentType, string content, Encoding encoding)
        {
            WriteDelimiter();
            WriteContentTypeHeader(contentType);
            WriteBytes(encoding.GetBytes(content));
        }

        public void WritePart(string contentType, byte[] content)
        {
            WriteDelimiter();
            WriteContentTypeHeader(contentType);
            WriteBytes(content);
        }

        public void WritePart (IEnumerable<HeaderField> headers, byte[] content)
        {
            WriteDelimiter();
            WriteHeaders(headers);
            WriteBytes(content);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (false == disposed)
            {
                // clean native resources         

                if (disposing)
                {
                    // clean managed resources       
                    WriteCloseDelimiter();
                    binaryWriter.Close();
                }

                disposed = true;
            }
        }

        private void WriteHeaders(IEnumerable<HeaderField> headers)
        {
            WriteCRLF();
            foreach (HeaderField header in headers)
            {
                WriteBytes(baseEncoding.GetBytes(header.ToString()));
                WriteCRLF();
            }

            WriteCRLF();
        }

        private void WriteContentTypeHeader(string contentType)
        {
            StringBuilder header = new StringBuilder();
            header.AppendFormat("\r\n{0}: {1}\r\n\r\n", HttpConstants.ContentType, contentType);
            WriteBytes(baseEncoding.GetBytes(header.ToString()));
        }

        private void WriteDelimiter()
        {
            string footer = "\r\n--" + boundary;
            WriteBytes(baseEncoding.GetBytes(footer));
        }

        private void WriteCloseDelimiter()
        {
            WriteDelimiter();
            WriteBytes(baseEncoding.GetBytes("--"));
        }

        private void WriteCRLF()
        {
            WriteBytes(new byte[] { (byte)'\r', (byte)'\n' });
        }

        private void WriteBytes(byte[] bytes)
        {
            binaryWriter.Write(bytes);
        }

        private bool disposed;
        private readonly string boundary;
        private readonly Encoding baseEncoding;
        private BinaryWriter binaryWriter;
    }
}