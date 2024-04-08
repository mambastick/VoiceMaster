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

        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, ex.Message);
        }

        return Task.CompletedTask;
    }
}