using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Serilog;

namespace VoiceMaster.Handlers.Command.SlashCommand;

public class SlashCommandHandler
{
    public Task ExecuteHandlerAsync(SlashCommandsExtension sender, SlashCommandExecutedEventArgs args)
    {
        Log.Logger.Information("Пользователь " + args.Context.User.Username + " использовал команду " +
                               args.Context.Interaction.Data.Name);
        return Task.CompletedTask;
    }

    public Task InvokeHandlerAsync(SlashCommandsExtension sender, SlashCommandInvokedEventArgs args)
    {
        Log.Logger.Information("Пользователь " + args.Context.User.Username + " вызвал команду " +
                               args.Context.Interaction.Data.Name);
        return Task.CompletedTask;
    }

    public Task ErrorHandlerAsync(SlashCommandsExtension sender, SlashCommandErrorEventArgs args)
    {
        Log.Logger.Error(args.Exception, args.Exception.Message);
        return Task.CompletedTask;
    }
}