using System.Text;
using BinaryMessageCode;

namespace BinaryMessageCodecTests
{
    [TestFixture]
    public class CodecTests
    {
        private BinaryMessageCodec _codec;

        [SetUp]
        public void SetUp()
        {
            _codec = new BinaryMessageCodec();
        }

        private void PerformEncodeDecodeTest(Message? message)
        {
            var encoded = _codec.Encode(message);
            var decoded = _codec.Decode(encoded);

            Assert.AreEqual(message?.Headers, decoded.Headers);
            Assert.AreEqual(message?.Payload, decoded.Payload);
        }

        [Test]
        public void TestEncodeDecode()
        {
            var originalMessage = new Message
            {
                Headers = new Dictionary<string, string> { { "Header1", "Value1" }, { "Header2", "Value2" } },
                Payload = Encoding.ASCII.GetBytes("Test Payload")
            };

            PerformEncodeDecodeTest(originalMessage);
        }

        [Test]
        public void BasicTest()
        {
            var message = new Message
            {
                Headers = new Dictionary<string, string> { { "Header1", "Value1" } },
                Payload = Encoding.ASCII.GetBytes("Hello World")
            };

            PerformEncodeDecodeTest(message);
        }

        [Test]
        public void EmptyHeadersTest()
        {
            var message = new Message
            {
                Headers = new Dictionary<string, string>(),
                Payload = Encoding.ASCII.GetBytes("Test Payload")
            };

            PerformEncodeDecodeTest(message);
        }

        [Test]
        public void MultipleHeadersTest()
        {
            var message = new Message
            {
                Headers = new Dictionary<string, string> {
                    { "Header1", "Value1" },
                    { "Header2", "Value2" },
                    { "Header3", "Value3" }
                },
                Payload = Encoding.ASCII.GetBytes("Another Test Payload")
            };

            PerformEncodeDecodeTest(message);
        }

        [Test]
        public void Encode_WithOversizedHeader_ThrowsException()
        {
            var message = new Message
            {
                Headers = new Dictionary<string, string> { { new string('a', 1024), "Value" } },
                Payload = Encoding.ASCII.GetBytes("Test Payload")
            };

            var ex = Assert.Throws<Exception>(() => _codec.Encode(message));
            Assert.That(ex.Message, Is.EqualTo("Header size exceeds the maximum allowed limit."));
        }

        [Test]
        public void Decode_WithInvalidDataFormat_ThrowsException()
        {
            var invalidData = new byte[] { 0xFF, 0xFF, 0xFF }; // Invalid data format

            // Expecting an EndOfStreamException to be thrown
            Assert.Throws<EndOfStreamException>(() => _codec.Decode(invalidData),
                "Decoding should fail with invalid data format.");
        }

        [Test]
        public void Encode_WithNullMessage_ThrowsException()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _codec.Encode(null));
            Assert.That(ex.ParamName, Is.EqualTo("message"));
        }
    }
}
