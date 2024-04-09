using dotenv.net;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Discord;
using Serilog.Sinks.SystemConsole.Themes;

namespace VoiceMaster;

public class Program
{
    private static async Task Main()
    {
        // Загрузка переменных сред
        var env = AppSettings.GetAppSettings();
        
        // Создание логгера
        Log.Logger = new LoggerConfiguration()
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
                    webhookId: Convert.ToUInt64(env["WEBHOOK_ID"]),
                    webhookToken: env["WEBHOOK_URL"],
                    restrictedToMinimumLevel: LogEventLevel.Information
                ); // Логируем в Discord
            })
            .CreateLogger();
        
        // Создаем бота
        var voiceMaster = new Bot(token: env["BOT_TOKEN"]);
        
        // Запускаем бота
        await voiceMaster.StartAsync();
        
        await Log.CloseAndFlushAsync();
    }
}