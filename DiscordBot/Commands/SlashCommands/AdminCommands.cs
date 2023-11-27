using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace DiscordBot.Commands.SlashCommands;

public class AdminCommands : ApplicationCommandModule
{
    [SlashCommand("activity", "Set activity for bot")]
    public async Task ActivityCommand(InteractionContext context,
        [Option("status", "Choose the bot's status")]
        UserStatus status,
        [Option("activity", "Choose the bot's activity type")]
        ActivityType activityType,
        [Option("activity-description", "Set the activity description")]
        string activityName)
    {
        try
        {
            var client = context.Client;
            await client.UpdateStatusAsync(new DiscordActivity(activityName, activityType), status);
            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent($"Активность бота успешно изменена на {activityName} со статусом {status}")
                    .AsEphemeral());
        }
        catch (Exception ex)
        {
            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent($"Произошла ошибка: {ex.Message}")
                    .AsEphemeral());
        }
    }
}