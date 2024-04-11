using System.Reflection;
using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Serilog;
using VoiceMaster.Commands.SlashCommands;

namespace VoiceMaster.Handlers;

public class InteractionCreatedHandler
{
    public Task ClientOnComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs args)
    {
        try
        {
            _ = Task.Run(async () =>
            {
                Log.Logger.Information($"User have been pressed button {args.Id}");
                switch (args.Id)
                {
                    case "create_setup_channel":
                        var interactionContext = new InteractionContext();

                        var interactionProperties = interactionContext.GetType()
                            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                            .Where(prop => prop.CanWrite && prop.DeclaringType == typeof(InteractionContext));

                        foreach (var prop in interactionProperties)
                        {
                            // TODO: Пофиксить баг с заполнением данных по полям класса
                            var argProp = args.GetType().GetProperty(prop.Name);
                            if (argProp == null || argProp.PropertyType != prop.PropertyType) continue;
                            var value = argProp.GetValue(args);
                            prop.SetValue(interactionContext, value);
                        }

                        await new SetupCommand().SetupCommandAsync(interactionContext);
                        return Task.CompletedTask;
                }

                return Task.CompletedTask;
            });
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, ex.Message);
            throw;
        }

        return Task.CompletedTask;
    }
}