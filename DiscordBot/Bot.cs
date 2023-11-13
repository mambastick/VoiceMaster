using Database;
using DiscordBot.Handlers;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using LoggerService;
using Microsoft.Extensions.Logging;

namespace DiscordBot;

public class Bot
{
    public Bot(string token, Logger logger, MySqlDatabase database)
    {
        Logger = logger;
        Logger.LogProcess("Инициализация бота...");

        Token = token;
        Database = database;

        Logger.LogSuccess("Бот успешно инициализирован.");
    }

    private string Token { get; }
    public static Logger Logger { get; set; }
    public static MySqlDatabase Database { get; set; }
    private static DiscordClient Client { get; set; }

    public async Task StartAsync()
    {
        Logger.LogProcess("Запуск бота...");

        var botConfig = new DiscordConfiguration
        {
            Token = Token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.Guilds | DiscordIntents.GuildVoiceStates,
            AutoReconnect = true,
            MinimumLogLevel = LogLevel.None
        };

        Client = new DiscordClient(botConfig);

        Client.Ready += Ready.ClientReady;
        Client.GuildAvailable += GuildAvailable.ClientGuildAvailable;
        Client.ClientErrored += Error.ClientError;
        Client.VoiceStateUpdated += VoiceState.VoiceStateUpdatedAsync;
        
        var activity = new DiscordActivity("Creating voice channels", ActivityType.Playing);

        await Client.ConnectAsync(activity, UserStatus.Online);

        Logger.LogSuccess("Бот успешно запущен!");
    }

    public async Task StopAsync() => await Client.DisconnectAsync();
}