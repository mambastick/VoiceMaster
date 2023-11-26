using DiscordBot.Data;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ChannelType = DiscordBot.Data.ChannelType;

namespace DiscordBot.Handlers;

public class VoiceState
{
    public static async Task VoiceStateHandlerAsync(DiscordClient sender, VoiceStateUpdateEventArgs e)
    {
        try
        {
            var createdVoiceChannelId = await new VoiceChannel().GetChannelIdAsync(e.Guild.Id, channelType: ChannelType.Setup);
            if (e.After.Channel != null && e.After.Channel.Id == createdVoiceChannelId)
                await new VoiceState().CreateVoiceChannelAsync(e);
            else if (e.Before?.Channel != null && e.Before.Channel.Id != createdVoiceChannelId
                     && e.Before.Channel.Users.Count == 0)
                await new VoiceState().DeleteVoiceChannelAsync(e);
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.ToString());
        }
    }

    private async Task CreateVoiceChannelAsync(VoiceStateUpdateEventArgs e)
    {
        try
        {
            var member = e.User as DiscordMember;
            var databaseChannel = new VoiceChannel();
            var voiceChannelId = await databaseChannel.GetChannelIdAsync(e.Guild.Id, e.User.Id);
            if (voiceChannelId != null)
            {
                var voiceChannel = e.Guild.GetChannel((ulong)voiceChannelId);
                await member.ModifyAsync(properties => properties.VoiceChannel = voiceChannel);
                
                Bot.Logger.LogInformation(
                    $"Пользователь {member.DisplayName} перемещен в голосовой канал " +
                    $"{member.VoiceState?.Channel?.Name} ({member.VoiceState?.Channel?.Id}) " +
                    $"(Попытка создать второй канал).");
            }
            else
            {
                var createdChannel = await e.Guild.CreateChannelAsync(
                    $"Канал - {member?.DisplayName}",
                    DSharpPlus.ChannelType.Voice,
                    e.Channel.Parent,
                    reason: $"Пользователь {member.DisplayName} создал голосовой канал.");

                await member.ModifyAsync(properties => properties.VoiceChannel = createdChannel);

                await databaseChannel.AddToDatabaseAsync(createdChannel, ChannelType.Usual, e.User.Id);

                Bot.Logger.LogInformation(
                    $"Пользователь {member.DisplayName} создал голосовой канал {member.VoiceState?.Channel?.Name} ({member.VoiceState?.Channel?.Id}).");
            }
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.ToString());
        }
    }

    private async Task DeleteVoiceChannelAsync(VoiceStateUpdateEventArgs e)
    {
        try
        {
            var voiceChannel = e.Before.Channel;
            await voiceChannel.DeleteAsync("Пользователь удалил голосовой канал.");

            await new VoiceChannel().DeleteFromDatabaseAsync(voiceChannel);

            var member = e.Before.User as DiscordMember;
            Bot.Logger.LogInformation(
                $"Пользователь {member.DisplayName} удалил голосовой канал {voiceChannel.Name} ({voiceChannel.Id}).");
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.ToString());
        }
    }
}