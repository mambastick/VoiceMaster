using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Serilog;
using VoiceMaster.Models;

namespace VoiceMaster.Commands.SlashCommands;

public class SetupCommand : ApplicationCommandModule
{
    [SlashCommand("setup", "Создать голосовой канал для создания временных голосовых каналов")]
    public async Task SetupCommandAsync(InteractionContext context)
    {
        try
        {
            var voiceChannel = await new SetupChannel().GetAsync(context.Guild.Id);
            if (voiceChannel is not null)
                throw new Exception("Канал для создания временных голосовых каналов уже установлен.");

            var parent = await context.Guild.CreateChannelCategoryAsync("Voice Master");
            var setupVoiceChannel = await context.Guild.CreateVoiceChannelAsync("Создать канал", parent);

            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent("Канал для создания временных голосовых каналов был успешно создан.\n" +
                                 $"Чтобы войти в голосовой канал, нажмите здесь: {setupVoiceChannel.Mention}\n\n" +
                                 "Вы также можете переместить этот канал в любое удобное для вас место, а также изменить его название.")
                    .AsEphemeral());

            voiceChannel = new SetupChannel
            {
                ChannelId = setupVoiceChannel.Id,
                GuildId = setupVoiceChannel.Guild.Id
            };
            await voiceChannel.AddAsync();

            var user = context.User as DiscordMember;
            Log.Logger.Information($"Пользователь {user.DisplayName} ({user.Id}) успешно создал создающий канал.");
        }
        catch (Exception ex) when (ex.Message == "Канал для создания временных голосовых каналов уже установлен.")
        {
            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent(ex.Message)
                    .AsEphemeral());
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, ex.Message);
            await context.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent("Произошла ошибка, канал для создания временных голосовых каналов не был создан.")
                    .AsEphemeral());
        }
    }
}