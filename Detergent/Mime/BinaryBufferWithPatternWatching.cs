using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using log4net;

namespace Detergent.Mime
{
    public class BinaryBufferWithPatternWatching : IDisposable
    {
        public BinaryBufferWithPatternWatching(byte[] pattern)
        {
            this.pattern = pattern;
            lastBytesBuffer = new byte[pattern.Length];
        }

        [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public byte[] Pattern
        {
            get { return pattern; }
        }

        public void Start()
        {
            if (bufferStream != null)
                bufferStream.Dispose();

            bufferStream = new MemoryStream();
            bytesCounter = 0;
        }

        public bool WriteByte (byte value)
        {
            bufferStream.WriteByte(value);

            lastBytesBuffer[bytesCounter%lastBytesBuffer.Length] = value;

            bytesCounter++;
            return MatchPatterns();
        }

        public byte[] ToArray()
        {
            return bufferStream.ToArray();
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
                    if (bufferStream != null)
                        bufferStream.Dispose();
                }

                disposed = true;
            }
        }

        private bool MatchPatterns()
        {
            if (bytesCounter < pattern.Length)
                return false;

            for (int i = 0; i < pattern.Length; i++)
            {
                byte actualByte = lastBytesBuffer[(bytesCounter - pattern.Length + i) % lastBytesBuffer.Length];
                byte expectedByte = pattern[i];
                if (actualByte != expectedByte)
                    return false;
            }

            return true;
        }

        private MemoryStream bufferStream;
        private bool disposed;
        private byte[] lastBytesBuffer;
        private long bytesCounter;
        private readonly byte[] pattern;
        //private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}