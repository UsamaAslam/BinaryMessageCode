using System.Text;
using BinaryMessageCode.Constants;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace BinaryMessageCode
{
    class Program
    {
        static void Main()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            LoggingConfig.ConfigureLogger(configuration);
            Console.WriteLine(MessageConstants.StartMessage);

            try
            {
                var codec = new BinaryMessageCodec();
                var message = new Message
                {
                    Headers = new Dictionary<string, string> { { "Header1", "Value1" } },
                    Payload = Encoding.ASCII.GetBytes("Hello World")
                };

                var encoded = codec.Encode(message);
                codec.Decode(encoded);

                Console.WriteLine(MessageConstants.StartSuccessfullMessage);
            }
            catch (Exception ex)
            {
                Log.Error(ex, MessageConstants.StartUnsuccessfullMessage);
            }
        }
    }
}