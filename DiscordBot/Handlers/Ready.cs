using DSharpPlus;
using DSharpPlus.EventArgs;

namespace DiscordBot.Handlers;

public static class Ready
{
    public static Task ReadyHandler(DiscordClient sender, ReadyEventArgs e)
    {
        Bot.Logger.LogSuccess("Бот готов к получению событий.");
        return Task.CompletedTask;
    }
}