using DiscordBot.Handlers;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Commands.SlashCommands;

public class AboutMeCommands : ApplicationCommandModule
{
    [SlashCommand("about-me", "Get information about me")]
    public async Task AboutMeCommandAsync(InteractionContext context)
    {
        try
        {
            var author = await Bot.Client.GetUserAsync(308647516965044226); // mambastick id (developer)
            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .AddEmbed(new DiscordEmbedBuilder()
                        .WithTitle("About me")
                        .WithAuthor(author.Username, "https://github.com/mambastick/", author.AvatarUrl)
                        .WithTimestamp(Convert.ToDateTime(DateTime.Now.ToString("dddd, HH:mm")))
                        .WithDescription("Hello, I'm glad you're using my bot.\n" +
                            "I created it to enrich my portfolio, and it's entirely free.\n\n" +
                            "I'll be actively fixing bugs and rolling out updates regularly to improve its functionality.\n" +
                            "Feel free to explore its features, and if you have any suggestions or encounter any issues, " +
                            "don't hesitate to let me know.\n" +
                            "Let's work together to make it even better!")
                        .WithColor(DiscordColor.Blue))
                    .AddComponents(new DiscordLinkButtonComponent("https://github.com/mambastick/VoiceMaster",
                        "My GitHub"))
                    .AsEphemeral());
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.ToString());
        }
    }
}