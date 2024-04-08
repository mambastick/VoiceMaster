using Serilog;
using Serilog.Events;
using Serilog.Sinks.Discord;
using Serilog.Sinks.SystemConsole.Themes;

namespace VoiceMaster;

public class Program
{
    private static async Task Main()
    {
        // Создание логгера
        var logger = Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(
                theme: SystemConsoleTheme.Colored,
                restrictedToMinimumLevel: LogEventLevel.Information,
                outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level:u}] {Message:lj}{NewLine}{Exception}"
            ) // Логируем в косноль
            .WriteTo.File(
                path: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs",
                    $"{DateTime.Now:dd-MM-yyyy HH:mm:ss}.log"),
                outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level:u}] {Message:lj}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Information
            ) // Логируем в файл
            .WriteTo.Async(a =>
            {
                a.Discord(
                    webhookId: 1226968570941145319,
                    webhookToken: "BHOBr0ixfRYulZpH5mizdQZ56trekZwSzWaopOLQdMYRhr_ogavq0I4uoUZTQJPoWZSC",
                    restrictedToMinimumLevel: LogEventLevel.Information
                ); // Логируем в Discord
            })
            .CreateLogger();
        
        
    }
}