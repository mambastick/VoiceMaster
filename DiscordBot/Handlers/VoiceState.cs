using System.Text.Json;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DiscordBot.Handlers;

public class VoiceState
{
    private static readonly string CreatedVoiceChannelName = "🔊│Создать канал";
    private static readonly Dictionary<ulong, ulong> VoiceChannels = new();

    private async Task CreateVoiceChannelAsync(DiscordClient sender, VoiceStateUpdateEventArgs e)
    {
        try
        {            
            var member = e.User as DiscordMember;
            var createdChannel = await e.Guild.CreateChannelAsync(
                name: $"Канал - {member?.DisplayName}",
                type: ChannelType.Voice,
                parent: e.Channel.Parent,
                reason: $"Пользователь {member.DisplayName} создал голосовой канал.");
            createdChannel.PlaceMemberAsync(member);
            VoiceChannels.Add(createdChannel.Id, member.Id);
            
            Bot.Logger.LogInformation($"Пользователь {member.DisplayName} создал голосовой канал.");
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.Message);
        }
    }

    private async Task DeleteVoiceChannelAsync(DiscordClient sender, VoiceStateUpdateEventArgs e)
    {
        try
        {
            if (VoiceChannels.Any(channel => channel.Key == e.Before.Channel.Id))
            {
                VoiceChannels.Remove(e.Before.Channel.Id);
                var deleteChannel = sender.GetChannelAsync(e.Before.Channel.Id);
                var member = e.User as DiscordMember;
                await deleteChannel.Result.DeleteAsync($"Пользователь {member.DisplayName} удалил голосовой канал.");
                Bot.Logger.LogInformation($"Пользователь {member.DisplayName} удалил голосовой канал.");
            }
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.ToString());
        }
    }

    public static async Task VoiceStateUpdatedAsync(DiscordClient sender, VoiceStateUpdateEventArgs e)
    {
        try
        {
            if (e.After.Channel != null && e.After.Channel.Name == CreatedVoiceChannelName)
                await new VoiceState().CreateVoiceChannelAsync(sender, e);
            else if (e.Before?.Channel != null && e.Before.Channel.Users.Count == 0 && e.Before.Channel.Name != CreatedVoiceChannelName)
                await new VoiceState().DeleteVoiceChannelAsync(sender, e);
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.Message);
        }
    }

}