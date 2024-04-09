using DSharpPlus;
using DSharpPlus.EventArgs;

namespace VoiceMaster.Handlers;

public class ChannelDeletedHandler
{
    public Task ClientOnChannelDeleted(DiscordClient sender, ChannelDeleteEventArgs args)
    {
        return Task.CompletedTask;
    }
}