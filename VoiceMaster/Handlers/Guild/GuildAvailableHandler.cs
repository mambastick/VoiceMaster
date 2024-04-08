using DSharpPlus;
using DSharpPlus.EventArgs;
using Serilog;

namespace VoiceMaster.Handlers.Guild;

public class GuildAvailableHandler
{
    public Task ClientOnGuildAvailable(DiscordClient sender, GuildCreateEventArgs args)
    {
        try
        {
            Log.Logger.Information($"Доступен сервер: {args.Guild.Name} ({args.Guild.Id})");
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, ex.Message);
            throw;
        }
        
        return Task.CompletedTask;
    }
}