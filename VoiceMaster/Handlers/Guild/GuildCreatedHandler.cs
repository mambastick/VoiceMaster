using DSharpPlus;
using DSharpPlus.EventArgs;

namespace VoiceMaster.Handlers.Guild;

public class GuildCreatedHandler
{
    public Task ClientOnGuildCreated(DiscordClient sender, GuildCreateEventArgs args)
    {
        return Task.CompletedTask;
    }
}