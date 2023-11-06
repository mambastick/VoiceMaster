using Database;
using DiscordBot.Handlers;
using DSharpPlus;
using DSharpPlus.Entities;
using LoggerService;

namespace DiscordBot;

public class Bot
{
    private string Token { get; set; }
    public static Logger Logger { get; set; }
    public static MySqlDatabase Database { get; set; }
    private static DiscordClient Client { get; set; }

    public Bot(string token, Logger logger, MySqlDatabase database)
    {
        Logger.LogProcess("Инициализация бота...");
        
        Token = token;
        Logger = logger;
        Database = database;
        
        Logger.LogSuccess("Бот успешно инициализирован.");
    }
    
    public async Task StartAsync()
    {
        Logger.LogProcess("Запуск бота...");
        
        var botConfig = new DiscordConfiguration()
        {
            Token = Token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.GuildVoiceStates,
            AutoReconnect = true,
            LogUnknownEvents = false,
            LoggerFactory = null
        };

        Client = new DiscordClient(botConfig);

        Client.Ready += Ready.ClientReady;
        Client.GuildAvailable += GuildAvailable.ClientGuildAvailable;
        Client.ClientErrored += Error.ClientError;
        Client.VoiceStateUpdated += VoiceState.VoiceStateUpdatedAsync;

        var activity = new DiscordActivity("Create voice channels", ActivityType.Playing);

        await Client.ConnectAsync(activity, UserStatus.Online);
        
        Logger.LogSuccess($"Бот успешно запущен!");
    }

    public async Task StopAsync() => await Client.DisconnectAsync();
}