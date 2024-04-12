using DSharpPlus;
using DSharpPlus.EventArgs;
using Serilog;

namespace VoiceMaster.Handlers.Error;

public class ErrorHandler
{
    public Task ClientOnError(DiscordClient client, ClientErrorEventArgs args)
    {
        try
        {
            Log.Logger.Error(args.Exception, args.Exception.Message);
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, ex.Message);
            throw;
        }
        
        return Task.CompletedTask;
    }
}