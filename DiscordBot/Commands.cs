using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot;

public abstract class Commands : ApplicationCommandModule
{
    [SlashCommand("setup", "Setup voice channel for creating temporary voice channels")]
    public async Task SetupCommandAsync(InteractionContext ctx)
    {
        try
        {
            var parent = await ctx.Guild.CreateChannelCategoryAsync("Voice Master");
            var setupVoiceChannel = await ctx.Guild.CreateVoiceChannelAsync("Create channel", parent);
                        
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                .WithContent($"Voice channel for creating temporary voice channels have been created.\n" +
                    $"Just join and create your own voice channel! {setupVoiceChannel.Mention}")
                .AsEphemeral());
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.ToString());
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                .WithContent($"Error: {ex.Message}")
                .AsEphemeral());
        }
    }
}