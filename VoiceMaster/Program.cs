using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using Polly;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Discord;
using Serilog.Sinks.SystemConsole.Themes;
using VoiceMaster.Database;

namespace VoiceMaster
{
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
                ) // Логируем в консоль
                .WriteTo.File(
                    path: $"logs/{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.log",
                    outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level:u}] {Message:lj}{NewLine}{Exception}",
                    restrictedToMinimumLevel: LogEventLevel.Information
                ) // Логируем в файл
                .WriteTo.Async(a =>
                {
                    a.Discord(
                        webhookId: Convert.ToUInt64(env["WEBHOOK_ID"]),
                        webhookToken: Convert.ToString(env["WEBHOOK_URL"]),
                        restrictedToMinimumLevel: LogEventLevel.Information
                    ); // Логируем в Discord
                })
                .CreateLogger();

            // Политика ретрая для ожидания доступности базы данных
            var retryPolicy = Policy
                .Handle<MySqlException>()
                .WaitAndRetryAsync(new[]
                {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(20),
                });

            // Подключение к базе данных с повторными попытками
            await retryPolicy.ExecuteAsync(async () =>
            {
                await using var context = new ApplicationContext();
                await context.Database.MigrateAsync();
                Log.Information("Успешно подключились к базе данных.");
            });

            // Создаем бота
            var voiceMaster = new Bot(token: Convert.ToString(env["BOT_TOKEN"]));

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
}