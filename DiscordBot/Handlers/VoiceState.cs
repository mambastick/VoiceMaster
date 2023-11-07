using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DiscordBot.Handlers;

public class VoiceState
{
    private const string CreatedVoiceChannelName = "🔊│Создать канал";
    private static readonly Dictionary<ulong, DiscordChannel> VoiceChannels = new();

    public static Task VoiceStateUpdatedAsync(DiscordClient sender, VoiceStateUpdateEventArgs e)
    {
        try
        {
            if (e.After.Channel != null && e.After.Channel.Name == CreatedVoiceChannelName)
                Task.Run(async () => await new VoiceState().CreateVoiceChannelAsync(e));
            else if (e.Before?.Channel != null && e.Before.Channel.Users.Count == 0 &&
                     e.Before.Channel.Name != CreatedVoiceChannelName)
                Task.Run(async () => await new VoiceState().DeleteVoiceChannelAsync(e));
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.ToString());
        }

        return Task.CompletedTask;
    }

    private async Task CreateVoiceChannelAsync(VoiceStateUpdateEventArgs e)
    {
        try
        {
            var member = e.User as DiscordMember;
            var parent = e.Channel.Parent;
            var existingChannel = VoiceChannels.GetValueOrDefault(e.User.Id);

            if (existingChannel != null)
                await member.ModifyAsync(properties => properties.VoiceChannel = existingChannel);
            else
            {
                var createdChannel = await e.Guild.CreateChannelAsync(
                    name: $"Канал - {member?.DisplayName}",
                    type: ChannelType.Voice,
                    parent: parent,
                    reason: $"Пользователь {member.DisplayName} создал голосовой канал.");
                VoiceChannels[e.User.Id] = createdChannel;

                await member.ModifyAsync(properties => properties.VoiceChannel = createdChannel);

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
            VoiceChannels.Remove(VoiceChannels.FirstOrDefault(findChannel => findChannel.Value == voiceChannel).Key);
            
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