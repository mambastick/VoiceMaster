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
                path: $"logs/{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.log",
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

        try
        {
            // Запускаем бота
            await voiceMaster.StartAsync();
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, ex.Message);
            throw;
        }
        finally
        {
            // Останавливаем бота
            await voiceMaster.StopAsync();

            // Закрываем логгер и очищаем память
            await Log.CloseAndFlushAsync();
        }
    }
}