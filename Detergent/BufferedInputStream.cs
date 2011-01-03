using System.IO;
using System.Text;

namespace Detergent
{
    /// <summary>
    /// Represents a <see cref="MemoryStream"/> that acts as a wrapper to some other input stream
    /// by loading itself with the complete contents of the input stream.
    /// </summary>
    public class BufferedInputStream : MemoryStream
    {
        public void BufferStream(Stream streamToBuffer)
        {
            this.streamToBuffer = streamToBuffer;
            using (BinaryReader reader = new BinaryReader(streamToBuffer))
            {
                BinaryWriter writer = new BinaryWriter(this);
                while (true)
                {
                    byte[] buffer = new byte[10000];
                    int bytesRead = reader.Read(buffer, 0, buffer.Length);
                    writer.Write(buffer, 0, bytesRead);
                    if (bytesRead == 0)
                        break;
                }
            }

            Seek(0, SeekOrigin.Begin);
        }

        public string DataToString(Encoding encoding)
        {
            return encoding.GetString(ToArray());
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (streamToBuffer != null)
                streamToBuffer.Dispose();
        }

        private Stream streamToBuffer;
    }
}