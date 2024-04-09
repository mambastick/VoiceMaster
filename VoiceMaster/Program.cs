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
        // Загрузка переменных среды из файла .env
        DotEnv.Load();

        // Проверка переменных сред
        ValidateEnvironment(DotEnv.Read());
        
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
                    webhookId: Convert.ToUInt64( Environment.GetEnvironmentVariable("WEBHOOK_ID")),
                    webhookToken: Environment.GetEnvironmentVariable("WEBHOOK_URL"),
                    restrictedToMinimumLevel: LogEventLevel.Information
                ); // Логируем в Discord
            })
            .CreateLogger();

        var voiceMaster = new Bot(token: Environment.GetEnvironmentVariable("TOKEN"));
        await voiceMaster.StartAsync();
    }
    
    private static void ValidateEnvironment(IDictionary<string, string> env)
    {
        foreach (var key in env.Keys)
        {
            var value = env[key];
            if (string.IsNullOrEmpty(value))
            {
                throw new Exception($"{key} не может быть пустым!");
            }
        }
    }
}