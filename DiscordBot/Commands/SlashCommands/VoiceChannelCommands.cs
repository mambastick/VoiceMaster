using DiscordBot.Data;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ChannelType = DiscordBot.Data.ChannelType;

namespace DiscordBot.Commands.SlashCommands;

public class VoiceChannelCommands : ApplicationCommandModule
{
    [SlashCommand("setup", "Create a voice channel for temporary voice channels")]
    public async Task SetupCommandAsync(InteractionContext context)
    {
        try
        {
            var voiceChannel = new VoiceChannel();
            var setupChannelExist = await voiceChannel.GetChannelIdAsync(context.Guild.Id,
                channelType: ChannelType.Setup);
            if (setupChannelExist is not null)
                throw new Exception("The setup channel has already been created.");

            var parent = await context.Guild.CreateChannelCategoryAsync("Voice Master");
            var setupVoiceChannel = await context.Guild.CreateVoiceChannelAsync("Create channel", parent);

            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent("The channel for creating temporary voice channels has been successfully created.\n" +
                        $"To enter the voice channel, click here: {setupVoiceChannel.Mention}\n\n" +
                        $"You can also move this channel to any place convenient for you, as well as change its name.")
                    .AsEphemeral());

            await voiceChannel.AddToDatabaseAsync(setupVoiceChannel, ChannelType.Setup);
        }
        catch (Exception ex) when (ex.Message == "The setup channel has already been created.")
        {
            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent(ex.Message)
                    .AsEphemeral());
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.ToString());
            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent("Some error has occurred, the action has not been performed.")
                    .AsEphemeral());
        }
    }
}