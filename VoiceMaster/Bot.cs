using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using Serilog;
using VoiceMaster.Handlers.Guild;
using VoiceMaster.Commands.SlashCommands;
using VoiceMaster.Handlers.Channel;
using VoiceMaster.Handlers.Command.SlashCommand;
using VoiceMaster.Handlers.Error;
using VoiceMaster.Handlers.Interaction;
using VoiceMaster.Handlers.Ready;
using VoiceMaster.Handlers.VoiceState;

namespace VoiceMaster;

public class Bot(string token)
{
    // Уникальный токен бота
    private string Token { get; set; } = token;
    
    // Клиент бота для действий (отправить сообщение, забанить и т.д.)
    public DiscordClient? Client { get; set; }
    
    // Токен, с помощью которого можно отменить действия
    private CancellationTokenSource? CancellationTokenSource { get; set; }
    
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
                | DiscordIntents.GuildIntegrations // Кнопки, формы и т.д.
                | DiscordIntents.GuildVoiceStates, // Действия пользователя в голосовом канале
            
            AutoReconnect = true, // Разрешаем автопереподключения в случае ошибки или т.п.
            
            MinimumLogLevel = LogLevel.Error, // Устанавливаем минимальный уровень логгирование для бота
            LoggerFactory = new LoggerFactory().AddSerilog(), // Используем Serilog в качесте логгера
            
            LogUnknownEvents = false // Отключаем логгирование неизвестных событий
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
        Client.Ready += new ReadyHandler().ClientOnReady; // Готовность бота
        Client.GuildAvailable += new GuildAvailableHandler().ClientOnGuildAvailable; // Доступные сервера
        Client.GuildCreated += new GuildCreatedHandler().ClientOnGuildCreated; // Создание сервера (т.е. добавление бота на него)
        Client.VoiceStateUpdated += new VoiceStateUpdateHandler().ClientOnVoiceStateUpdated;// Голосовые каналы
        Client.ChannelDeleted += new ChannelDeletedHandler().ClientOnChannelDeleted; // Удаление канала
        Client.ComponentInteractionCreated += new InteractionCreatedHandler().ClientOnComponentInteractionCreated; // Кнопки
        
        // Используем слэш-команды
        var slashCommands = Client.UseSlashCommands();
        slashCommands.SlashCommandErrored += new SlashCommandHandler().ErrorHandlerAsync;
        slashCommands.SlashCommandInvoked += new SlashCommandHandler().InvokeHandlerAsync;
        slashCommands.SlashCommandExecuted += new SlashCommandHandler().ExecuteHandlerAsync;
        slashCommands.RegisterCommands<SetupCommand>();
        
        // Подключаемся к серверу для прослушивания событий
        await Client.ConnectAsync(
            activity: botActivity,
            status: UserStatus.Online
            );
        
        // Создаем токен отмены
        CancellationTokenSource = new CancellationTokenSource();
        
        // Запускаем бесконечный цикл с токеном отмены
        while (!CancellationTokenSource.IsCancellationRequested)
        {
            // Игнорируем
        }
    }

    // Остановка бота
    public async Task StopAsync()
    {
        Log.Logger.Information($"Бот выключается...");
        await Client.DisconnectAsync();
        await CancellationTokenSource.CancelAsync();
        Log.Logger.Information($"Бот выключен.");
    }
}