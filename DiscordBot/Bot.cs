using Database;
using DiscordBot.Commands.SlashCommands;
using DiscordBot.Handlers;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using LoggerService;
using Microsoft.Extensions.Logging;

namespace DiscordBot;

public class Bot
{
    public Bot(string token, Logger logger, MySqlDatabase database)
    {
        Logger = logger;
        Logger.LogProcess("Initializing the bot...");

        Token = token;
        Database = database;

        Logger.LogSuccess("The bot has been successfully initialized.");
    }

    private string Token { get; }
    public static Logger Logger { get; private set; }
    public static MySqlDatabase Database { get; private set; }
    public static DiscordClient Client { get; private set; }

    public async Task StartAsync()
    {
        Logger.LogProcess("Starting the bot...");

        var botConfig = new DiscordConfiguration
        {
            Token = Token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.Guilds
                | DiscordIntents.GuildVoiceStates
                | DiscordIntents.GuildIntegrations,
            AutoReconnect = true,
            MinimumLogLevel = LogLevel.None
        };

        Client = new DiscordClient(botConfig);

        Client.Ready += Ready.ReadyHandler;
        Client.GuildAvailable += GuildAvailable.GuildAvailableHandler;
        Client.ClientErrored += Error.ErrorHandler;
        Client.VoiceStateUpdated += VoiceState.VoiceStateHandlerAsync;
        Client.ChannelDeleted += Channel.ChannelDeletedHandlerAsync;
        Client.GuildCreated += Guild.GuildCreatedHandlerAsync;
        Client.ComponentInteractionCreated += Interaction.ButtonHandlerAsync;

        var slashCommands = Client.UseSlashCommands();
        slashCommands.SlashCommandErrored += SlashCommand.ErrorHandlerAsync;
        slashCommands.SlashCommandInvoked += SlashCommand.InvokeHandlerAsync;
        slashCommands.SlashCommandExecuted += SlashCommand.ExecuteHandlerAsync;
        slashCommands.RegisterCommands<VoiceChannelCommands>();
        slashCommands.RegisterCommands<AboutMeCommands>();
        slashCommands.RegisterCommands<AdminCommands>(645297558994026513);

        var activity = new DiscordActivity("Creating voice channels", ActivityType.Playing);

        await Client.ConnectAsync(activity, UserStatus.Online);

        Logger.LogSuccess("The bot is running.");
    }

    public async Task StopAsync() => await Client.DisconnectAsync();
}