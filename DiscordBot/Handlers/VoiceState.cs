using System.Text.Json;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace DiscordBot.Handlers;

public class VoiceState
{
    private static readonly string CreatedVoiceChannelName = "🔊│Создать канал";
    private static readonly Dictionary<string, DiscordChannel> VoiceChannels = new();

    public static Task VoiceStateUpdatedAsync(DiscordClient sender, VoiceStateUpdateEventArgs e)
    {
        try
        {
            if (e.After.Channel != null && e.After.Channel.Name == CreatedVoiceChannelName)
                Task.Run(async () => await new VoiceState().CreateVoiceChannelAsync(sender, e));
            else if (e.Before?.Channel != null && e.Before.Channel.Name != CreatedVoiceChannelName)
                Task.Run(async () => await new VoiceState().DeleteVoiceChannelAsync(sender, e));
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.ToString());
        }

        return Task.CompletedTask;
    }

    private async Task CreateVoiceChannelAsync(DiscordClient sender, VoiceStateUpdateEventArgs e)
    {
        try
        {
            if (!VoiceChannels.ContainsKey($"{e.Guild.Id}_{e.User.Id}"))
            {
                var member = e.User as DiscordMember;
                var parent = e.Channel.Parent;
                var existingChannel = VoiceChannels.GetValueOrDefault($"{e.Guild.Id}_{e.User.Id}");

                if (existingChannel != null)
                    await member.ModifyAsync(properties => properties.VoiceChannel = existingChannel);
                else
                {
                    var createdChannel = await e.Guild.CreateChannelAsync(
                        name: $"Канал - {member?.DisplayName}",
                        type: ChannelType.Voice,
                        parent: parent,
                        reason: $"Пользователь {member.DisplayName} создал голосовой канал.");
                    VoiceChannels[$"{e.Guild.Id}_{e.User.Id}"] = createdChannel;

                    await member.ModifyAsync(properties => properties.VoiceChannel = createdChannel);

                    Bot.Logger.LogInformation(
                        $"Пользователь {member.DisplayName} создал голосовой канал {member.VoiceState?.Channel?.Name} ({member.VoiceState?.Channel?.Id}).");
                }
            }
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.ToString());
        }
    }

    private async Task DeleteVoiceChannelAsync(DiscordClient sender, VoiceStateUpdateEventArgs e)
    {
        try
        {
            await e.Before.Channel.DeleteAsync("Пользователь удалил голосовой канал.");
            VoiceChannels.Remove($"{e.Guild.Id}_{e.User.Id}");

            var member = e.User as DiscordMember;
            Bot.Logger.LogInformation(
                $"Пользователь {member.DisplayName} удалил голосовой канал {e.Before.Channel.Name} ({e.Before.Channel.Id}).");
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.ToString());
        }
    }
}