using DSharpPlus;
using DSharpPlus.EventArgs;
using Serilog;

namespace VoiceMaster.Handlers;

public class ErrorHandler
{
    public Task ClientOnError(DiscordClient client, ClientErrorEventArgs args)
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