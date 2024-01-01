using System.Text;
using BinaryMessageCode.Constants;
using Serilog;

namespace BinaryMessageCode
{
    public class BinaryMessageCodec : IMessageCodec
    {
        public byte[] Encode(Message? message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), MessageConstants.MessageCannotBeNull);
            }

            try
            {
                using var memoryStream = new MemoryStream();
                using (var writer = new BinaryWriter(memoryStream, Encoding.ASCII))
                {
                    writer.Write((byte)message.Headers.Count);
                    foreach (var header in message.Headers)
                    {
                        ValidateAndWriteHeader(writer, header);
                    }
                    writer.Write(message.Payload.Length);
                    writer.Write(message.Payload);
                }
                Log.Information(MessageConstants.MessageEncodedSuccessfully);
                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.MessageEncodedUnsuccessfully);
                throw;
            }
        }

        public Message Decode(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), MessageConstants.DataCannotBeNull);
            }

            try
            {
                var message = new Message();
                using var memoryStream = new MemoryStream(data);
                using (var reader = new BinaryReader(memoryStream, Encoding.ASCII))
                {
                    var headersCount = reader.ReadByte();
                    for (int i = 0; i < headersCount; i++)
                    {
                        var key = ReadString(reader);
                        var value = ReadString(reader);
                        message.Headers.Add(key, value);
                    }
                    var payloadLength = reader.ReadInt32();
                    message.Payload = reader.ReadBytes(payloadLength);
                }
                Log.Information(MessageConstants.MessageEncodedSuccessfully);
                return message;
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.MessageEncodedUnsuccessfully);
                throw;
            }
        }

        private void ValidateAndWriteHeader(BinaryWriter writer, KeyValuePair<string, string> header)
        {
            var keyBytes = Encoding.ASCII.GetBytes(header.Key);
            var valueBytes = Encoding.ASCII.GetBytes(header.Value);

            if (keyBytes != null && (keyBytes.Length > 1023 || valueBytes.Length > 1023))
            {
                throw new Exception(MessageConstants.HeaderSizeExceeded);

            }

            if (keyBytes != null)
            {
                writer.Write((ushort)keyBytes.Length);
                writer.Write(keyBytes);
            }

            writer.Write((ushort)valueBytes.Length);
            writer.Write(valueBytes);
        }

        private string ReadString(BinaryReader reader)
        {
            var length = reader.ReadUInt16();
            var bytes = reader.ReadBytes(length);
            return Encoding.ASCII.GetString(bytes);
        }
    }
}
