using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DiscordBot.Handlers;

public class Guild
{
    public static async Task GuildCreatedHandlerAsync(DiscordClient client, GuildCreateEventArgs createdGuild)
    {
        try
        {
            Bot.Logger.LogInformation($"Bot was added in new guild: {createdGuild.Guild.Name}");
            
            var defaultChannel = createdGuild.Guild.GetDefaultChannel();

            var embedGettingStarted = new DiscordEmbedBuilder()
                .WithTitle("Getting started")
                .WithDescription("Hello, dear administrators.\n" +
                    "I'm glad you added me to your server.\n\n" +
                    "I was created to make temporary voice channels for users of your server.\n" +
                    "Just use Setup button.")
                .WithColor(DiscordColor.IndianRed);

            var helloMessage = new DiscordMessageBuilder()
                .WithEmbed(embedGettingStarted)
                .AddComponents(new List<DiscordComponent>()
                {
                    new DiscordButtonComponent(ButtonStyle.Success, 
                        "create_setup_channel",
                        "Setup", 
                        emoji: new DiscordComponentEmoji(DiscordEmoji.FromName(client, ":loud_sound:")))
                });
            
            await defaultChannel.SendMessageAsync(helloMessage);
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.ToString());
        }
    }
}