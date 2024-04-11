using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Serilog;

namespace VoiceMaster.Handlers.Guild;

public class GuildCreatedHandler
{
    public Task ClientOnGuildCreated(DiscordClient sender, GuildCreateEventArgs args)
    {
        try
        {
            _ = Task.Run(async () =>
            {
                Log.Logger.Information($"Бот был добавлен на новый сервер: {args.Guild.Name}");

                var defaultChannel = args.Guild.GetDefaultChannel();

                var embedGettingStarted = new DiscordEmbedBuilder()
                    .WithTitle("Начало работы")
                    .WithDescription("Привет, уважаемые администраторы.\n" +
                                     "Я рад, что вы добавили меня на свой сервер.\n\n" +
                                     "Меня создали для создания временных голосовых каналов для пользователей вашего сервера.\n" +
                                     "Просто используйте кнопку \"Создать создающий канал\".")
                    .WithColor(DiscordColor.IndianRed);

                var helloMessage = new DiscordMessageBuilder()
                    .WithEmbed(embedGettingStarted)
                    .AddComponents(new List<DiscordComponent>()
                    {
                        new DiscordButtonComponent(ButtonStyle.Success,
                            "create_setup_channel",
                            "Создать создающий канал",
                            emoji: new DiscordComponentEmoji(DiscordEmoji.FromName(sender, ":loud_sound:")))
                    });

                await defaultChannel.SendMessageAsync(helloMessage);
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