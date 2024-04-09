using DSharpPlus;
using DSharpPlus.EventArgs;

namespace VoiceMaster.Handlers;

public class VoiceStateUpdateHandler
{
    public Task ClientOnVoiceStateUpdated(DiscordClient sender, VoiceStateUpdateEventArgs args)
    {
        return Task.CompletedTask;
    }
}