using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;

namespace Bot.Handlers;

public static class GuildAvailable
{
    public static Task ClientGuildAvailable(DiscordClient sender, GuildCreateEventArgs e)
    {
        Bot.Logger.LogInformation($"Список серверов: {e.Guild.Name}");
        return Task.CompletedTask;
    }
}