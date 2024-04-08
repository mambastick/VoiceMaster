using DSharpPlus;
using DSharpPlus.EventArgs;
using Serilog;

namespace VoiceMaster.Handlers;

public class ReadyHandler
{
    public Task ClientOnReady(DiscordClient client, ReadyEventArgs args)
    {
        try
        {
            Log.Logger.Information($"Бот готов к получению приходящих событий.");
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, ex.Message);
            throw;
        }

        return Task.CompletedTask;
    }
}