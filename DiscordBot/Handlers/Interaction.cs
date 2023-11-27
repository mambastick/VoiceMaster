using DiscordBot.Data;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DiscordBot.Handlers;

public class Interaction
{
    public static async Task ButtonHandlerAsync(DiscordClient client, ComponentInteractionCreateEventArgs ctx)
    {
        try
        {
            _ = Task.Run(async () =>
            {
                Bot.Logger.LogInformation($"User have been pressed button {ctx.Id}");
                switch (ctx.Id)
                {
                    case "create_setup_channel":
                        var voiceChannel = new VoiceChannel();
                        var setupChannelExist = await voiceChannel.GetChannelIdAsync(ctx.Guild.Id,
                            channelType: Data.ChannelType.Setup);
                        if (setupChannelExist is not null)
                            throw new Exception("The setup channel has already been created.");

                        var parent = await ctx.Guild.CreateChannelCategoryAsync("Voice Master");
                        var setupVoiceChannel = await ctx.Guild.CreateVoiceChannelAsync("Create channel", parent);
                        await voiceChannel.AddToDatabaseAsync(setupVoiceChannel, Data.ChannelType.Setup);

                        var embedGettingStarted = new DiscordEmbedBuilder()
                            .WithTitle("Getting started")
                            .WithDescription("Hello, dear administrators.\n" +
                                "I'm glad you added me to your server.\n\n" +
                                "I was created to make temporary voice channels for users of your server.\n" +
                                "Just use Setup button.")
                            .WithColor(DiscordColor.IndianRed);

                        await ctx.Interaction.CreateResponseAsync(
                            InteractionResponseType.UpdateMessage,
                            new DiscordInteractionResponseBuilder()
                                .AddEmbed(embedGettingStarted)
                                .AddComponents(new DiscordButtonComponent(ButtonStyle.Success,
                                    "create_setup_channel",
                                    "Setup",
                                    true,
                                    new DiscordComponentEmoji(DiscordEmoji.FromName(client, ":loud_sound:")))));
                        return;
                }
            });
        }
        catch (Exception ex) when (ex.Message == "The setup channel has already been created.")
        {
            await ctx.Interaction.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent("The setup channel has already been created.")
                    .AsEphemeral());
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.ToString());
        }
    }
}