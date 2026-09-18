using Serilog;
using Serilog.Events;

namespace Todo.WebApi.Extensions;

public static class LoggingExtensions
{
    public static void AddFlexibleLogging(this WebApplicationBuilder builder)
    {
        var options = builder.Configuration.GetSection("LoggingOptions");
        var provider = options["Provider"] ?? "Console";
        var minLevelStr = options["MinimumLevel"] ?? "Information";

        builder.Logging.ClearProviders();

        switch (provider)
        {
            case "Serilog":
                var minLevel = Enum.TryParse<LogEventLevel>(minLevelStr, out var lvl)
                    ? lvl
                    : LogEventLevel.Information;

                var logFilePath = options["LogFilePath"] ?? "logs/app-.log";

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Is(minLevel)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                    .CreateLogger();

                builder.Host.UseSerilog();
                break;

            case "Console":
            default:
                builder.Logging.AddConsole();
                builder.Logging.AddDebug();
                break;
        }
    }
}