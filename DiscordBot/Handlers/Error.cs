using DSharpPlus;
using DSharpPlus.EventArgs;

namespace DiscordBot.Handlers;

public static class Error
{
    public static Task ErrorHandler(DiscordClient sender, ClientErrorEventArgs e)
    {
        Bot.Logger.LogError(e.Exception.Message);
        return Task.CompletedTask;
    }
}