using DSharpPlus;
using DSharpPlus.EventArgs;

namespace DiscordBot.Handlers;

public static class GuildAvailable
{
    public static Task ClientGuildAvailable(DiscordClient sender, GuildCreateEventArgs e)
    {
        Bot.Logger.LogInformation($"Сервер {e.Guild.Name} активен.");
        return Task.CompletedTask;
    }
}