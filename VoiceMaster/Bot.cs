using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Serilog;

namespace VoiceMaster;

public class Bot(string token)
{
    // Уникальный токен бота
    private string Token { get; set; } = token;
    
    // Клиент бота для действий (отправить сообщение, забанить и т.д.)
    public DiscordClient? Client { get; set; }
    
    // Запуск бота
    public async Task StartAsync()
    {
        // Создаем конфигурацию бота
        var botConfig = new DiscordConfiguration
        {
            Token = Token, // Токен
            TokenType = TokenType.Bot, // Тип токена: Бот или Пользователь
            
            // Список разрешенных событий
            Intents =
                DiscordIntents.Guilds // Сервера (где бот добавлен и т.д.)
                | DiscordIntents.GuildVoiceStates // Голосовая активность пользователей и ботов (зашел/вышел в голосовой канал)
                | DiscordIntents.GuildIntegrations, // Кнопки, формы и т.д.
            AutoReconnect = true, // Разрешаем автопереподключения в случае ошибки или т.п.
            MinimumLogLevel = LogLevel.Debug, // Устанавливаем минимальный уровень логирования на Debug
        };
        
        // Создаем клиент Discord с использованием конфигурации бота
        Client = new DiscordClient(botConfig);
        
        // Активность бота, что он делает: играет, смотрит или стримит ?
        var botActivity = new DiscordActivity()
        {
            Name = "Создание каналов",
            ActivityType = ActivityType.Playing,
        };

        // Прослушиваем события 
        Client.ClientErrored += new ErrorHandler().ClientOnError; // Ошибки
        // Подключаемся к серверу для прослушивания событий
        await Client.ConnectAsync(
            activity: botActivity,
            status: UserStatus.Online
            );
    }

    // Остановка бота
    public async Task StopAsync()
    {
        Log.Logger.Information($"Бот выключается...");
        await Client.DisconnectAsync();
        Log.Logger.Information($"Бот выключен.");
    }
}