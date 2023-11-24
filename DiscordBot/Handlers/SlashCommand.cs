using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;

namespace DiscordBot.Handlers;

public abstract class SlashCommand
{
    public static Task ExecuteHandlerAsync(SlashCommandsExtension sender, SlashCommandExecutedEventArgs args)
    {
        Bot.Logger.LogSuccess(args.Context.User.Username + " успешно использовал команду " +
            args.Context.Interaction.Data.Name);
        return Task.CompletedTask;
    }

    public static Task InvokeHandlerAsync(SlashCommandsExtension sender, SlashCommandInvokedEventArgs args)
    {
        Bot.Logger.LogProcess(args.Context.User.Username + " использует команду " +
            args.Context.Interaction.Data.Name);
        return Task.CompletedTask;
    }

    public static Task ErrorHandlerAsync(SlashCommandsExtension sender, SlashCommandErrorEventArgs args)
    {
        Bot.Logger.LogError(args.Exception.Message);
        return Task.CompletedTask;
    }
}