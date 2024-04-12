using System.Reflection;
using DSharpPlus;
using DSharpPlus.Entities;
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
                var user = args.User as DiscordMember;
                Log.Logger.Information($"Пользователь {user.DisplayName} ({user.Id}) нажал на кнопку {args.Id}");
                switch (args.Id)
                {
                    case "create_setup_channel":
                        var interactionContext = new InteractionContext();

                        var interactionProperties = interactionContext.GetType().GetProperties();

                        foreach (var prop in interactionProperties)
                        {
                            var argProp = args.GetType().GetProperty(prop.Name);
                            if (argProp == null || argProp.PropertyType != prop.PropertyType) continue;
                            var value = argProp.GetValue(args);
                            prop.SetValue(interactionContext, value);
                        }

                        var createSetupChannelTask = new SetupCommand().SetupCommandAsync(interactionContext);
                        var editMessageTask = Task.Run(async () =>
                        {
                            var embedGettingStarted = new DiscordEmbedBuilder()
                                .WithTitle("Начало работы")
                                .WithDescription("Привет, уважаемые администраторы.\n" +
                                                 "Я рад, что вы добавили меня на свой сервер.\n\n" +
                                                 "Меня создали для создания временных голосовых каналов для пользователей вашего сервера.\n" +
                                                 "Просто используйте кнопку \"Установить\".")
                                .WithColor(DiscordColor.IndianRed);

                            await args.Message.ModifyAsync(new DiscordMessageBuilder()
                                .AddEmbed(embedGettingStarted)
                                .AddComponents(new DiscordButtonComponent(ButtonStyle.Success,
                                    "createSetupChannel",
                                    "Установить",
                                    true,
                                    new DiscordComponentEmoji(DiscordEmoji.FromName(sender, ":loud_sound:")))));
                        });

                        await Task.WhenAll(createSetupChannelTask, editMessageTask);

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