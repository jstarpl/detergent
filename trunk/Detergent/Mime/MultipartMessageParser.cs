using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using log4net;

namespace Detergent.Mime
{
    public class MultipartMessageParser
    {
        public MultipartMessage ParseMessage(string contentType, Stream inputStream)
        {
            if (log.IsDebugEnabled)
                log.DebugFormat("Message content-type: '{0}'", contentType);

            HeaderField contentTypeField = ParseHeaderField(
                "Content-Type",
                contentType);

            MultipartMessage message = new MultipartMessage(contentTypeField);

            if (!message.IsMultipart)
                throw ParserException("This is not a MIME multipart message. The content type '{0}' is not supported", contentType);

            if (log.IsDebugEnabled)
                log.DebugFormat("Multipart message boundary: '{0}'", message.Boundary);

            BinaryBufferWithPatternWatching buffer = new BinaryBufferWithPatternWatching(
                ConstructDelimiterBuffer(message));

            try
            {
                SkipToFirstDelimiter(inputStream, buffer);
            
                while (true)
                {
                    int markerByte1 = inputStream.ReadByte();
                    int markerByte2 = inputStream.ReadByte();

                    if (markerByte1 == -1 || markerByte2 == -1)
                        throw ParserException("Unexpected end of stream (1).");

                    if ((byte)markerByte1 == '-' && (byte)markerByte2 == '-')
                        break;

                    if ((byte)markerByte1 == '\r' && (byte)markerByte2 == '\n')
                    {
                        MultipartMessagePart part = ReadPart(message, inputStream, buffer);
                        message.AddPart(part);
                    }
                    else
                        throw ParserException("Invalid multipart message: unexpected message part markers.");
                }

                return message;
            }
            catch (EndOfStreamException ex)
            {
                throw new MultipartMessageParserException("Unexpected end of stream.", ex);
            }
        }

        private static void SkipToFirstDelimiter(Stream inputStream, BinaryBufferWithPatternWatching buffer)
        {
            buffer.Start();

            while (true)
            {
                int value = inputStream.ReadByte();
                if (value == -1)
                    throw ParserException("Unexpected end of stream (2).");

                byte byteValue = (byte)value;
                if (buffer.WriteByte(byteValue))
                    return;
            }
        }

        private static MultipartMessagePart ReadPart(
            MultipartMessage message,
            Stream inputStream, 
            BinaryBufferWithPatternWatching buffer)
        {
            buffer.Start();

            while (true)
            {
                int value = inputStream.ReadByte();
                if (value == -1)
                    throw ParserException("Unexpected end of stream (3).");

                byte byteValue = (byte)value;
                bool patternMatched = buffer.WriteByte(byteValue);

                if (!patternMatched)
                    continue;

                MultipartMessagePart part = ParsePart(
                    message,
                    buffer.ToArray(), 
                    buffer.Pattern.Length);

                if (log.IsDebugEnabled)
                    log.DebugFormat("Found part '{0}'", message.BaseEncoding.GetString(part.Data));

                return part;
            }
        }

        private static byte[] ConstructDelimiterBuffer(MultipartMessage message)
        {
            string value = "\r\n--" + message.Boundary;
            return message.BaseEncoding.GetBytes(value);
        }

        private static HeaderField ParseHeaderField(string fieldName, string fieldValueComplete)
        {
            return ParseHeaderField(fieldName + " : " + fieldValueComplete);
        }

        private static HeaderField ParseHeaderField(string value)
        {
            return HeaderField.Parse(value);
        }

        private static MultipartMessagePart ParsePart(
            MultipartMessage message,
            byte[] data, 
            int excludedLastBytes)
        {
            using (MemoryStream stream = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                MultipartMessagePart part = ParsePartHeaders(reader, message);

                // copy the part data into a separate array
                byte[] partData = new byte[data.Length - excludedLastBytes - stream.Position];
                Array.Copy(data, stream.Position, partData, 0, partData.Length);
                part.Data = partData;
                return part;
            }
        }

        private static MultipartMessagePart ParsePartHeaders(BinaryReader reader, MultipartMessage message)
        {
            MultipartMessagePart part = new MultipartMessagePart();

            while (true)
            {
                byte[] bytes = reader.ReadUntilCRLF();

                // did we reach the end of the headers list?
                if (bytes.Length == 0)
                    break;

                HeaderField header = HeaderField.Parse(bytes, message.BaseEncoding);
                part.AddHeader(header);
            }

            return part;
        }

        private static MultipartMessageParserException ParserException(string format, params object[] args)
        {
            string message = string.Format(
                CultureInfo.InvariantCulture,
                format,
                args);
            return new MultipartMessageParserException(message);
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}