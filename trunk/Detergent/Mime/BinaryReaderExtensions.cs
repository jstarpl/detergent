using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace Detergent.Mime
{
    public static class BinaryReaderExtensions
    {
        /// <summary>
        /// Reads the data from the underlying stream until it finds a CRLF.
        /// </summary>
        /// <param name="reader">The binary reader.</param>
        /// <returns>
        /// A byte array containing data read from the underlying stream.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "CRLF")]
        public static byte[] ReadUntilCRLF(this BinaryReader reader)
        {
            List<byte> readData = new List<byte>();
            while (true)
            {
                byte b = reader.ReadByte();
                if (b == '\r')
                {
                    byte b2 = reader.ReadByte();
                    if (b2 == '\n')
                        return readData.ToArray();

                    readData.Add(b);
                    readData.Add(b2);
                }

                readData.Add(b);
            }
        }

        public static string ArrayToString (this Array array)
        {
            StringBuilder s = new StringBuilder();
            foreach (object value in array)
            {
                if (s.Length > 0)
                    s.Append(", ");
                s.Append(value);
            }

            return s.ToString();
        }
    }
}