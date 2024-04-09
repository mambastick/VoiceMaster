using DSharpPlus;
using DSharpPlus.EventArgs;

namespace VoiceMaster.Handlers;

public class InteractionCreatedHandler
{
    public Task ClientOnComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs args)
    {
        return Task.CompletedTask;
    }
}