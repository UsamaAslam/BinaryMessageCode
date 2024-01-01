using Microsoft.Extensions.Configuration;
using Serilog;

namespace BinaryMessageCode
{
    public static class LoggingConfig
    {
        public static void ConfigureLogger(IConfiguration configuration)
        {
            var logFilePath = configuration["CodecSettings:LogFilePath"];

            if (logFilePath != null)
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()
                    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                    .CreateLogger();
        }
    }
}