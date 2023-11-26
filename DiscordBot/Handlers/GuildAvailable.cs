using DSharpPlus;
using DSharpPlus.EventArgs;

namespace DiscordBot.Handlers;

public static class GuildAvailable
{
    public static Task ClientGuildAvailable(DiscordClient sender, GuildCreateEventArgs e)
    {
        Bot.Logger.LogInformation($"Guild {e.Guild.Name} available.");
        return Task.CompletedTask;
    }
}