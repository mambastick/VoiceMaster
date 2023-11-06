using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Bot.Handlers;

public static class Error
{
    public static Task ClientError(DiscordClient sender, ClientErrorEventArgs e)
    {
        Bot.Logger.LogError(e.Exception.Message);
        return Task.CompletedTask;
    }
}