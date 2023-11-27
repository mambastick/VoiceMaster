using DiscordBot.Data;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using ChannelType = DiscordBot.Data.ChannelType;

namespace DiscordBot.Handlers;

public class VoiceState
{
    public static Task VoiceStateHandlerAsync(DiscordClient sender, VoiceStateUpdateEventArgs e)
    {
        try
        {
            _ = Task.Run(async () =>
            {
                var createdVoiceChannelId =
                    await new VoiceChannel().GetChannelIdAsync(e.Guild.Id, channelType: ChannelType.Setup);
                if (e.After.Channel != null && e.After.Channel.Id == createdVoiceChannelId)
                    await new VoiceState().CreateVoiceChannelAsync(e);
                else if (e.Before?.Channel != null && e.Before.Channel.Id != createdVoiceChannelId
                         && e.Before.Channel.Users.Count == 0)
                    await new VoiceState().DeleteVoiceChannelAsync(e); 
            });
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
            var databaseChannel = new VoiceChannel();
            var voiceChannelId = await databaseChannel.GetChannelIdAsync(e.Guild.Id, e.User.Id);
            if (voiceChannelId != null)
            {
                var voiceChannel = e.Guild.GetChannel((ulong)voiceChannelId);
                await member.ModifyAsync(properties => properties.VoiceChannel = voiceChannel);

                Bot.Logger.LogInformation(
                    $"User {member.DisplayName} moved to the voice channel " +
                    $"{member.VoiceState?.Channel?.Name} ({member.VoiceState?.Channel?.Id}) " +
                    $"(Attempt to create a second channel).");
            }
            else
            {
                var createdChannel = await e.Guild.CreateChannelAsync(
                    $"Channel - {member?.DisplayName}",
                    DSharpPlus.ChannelType.Voice,
                    e.Channel.Parent,
                    reason: $"User {member.DisplayName} created a voice channel.");

                await member.ModifyAsync(properties => properties.VoiceChannel = createdChannel);

                await databaseChannel.AddToDatabaseAsync(createdChannel, ChannelType.Usual, e.User.Id);

                Bot.Logger.LogInformation(
                    $"User {member.DisplayName} ({member.Id}) created voice channel: {member.VoiceState?.Channel?.Name} " +
                    $"({member.VoiceState?.Channel?.Id}).");
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
            var user = e.User as DiscordMember;
            await voiceChannel.DeleteAsync($"User {user.DisplayName} deleted voice channel");

            await new VoiceChannel().DeleteFromDatabaseAsync(voiceChannel);
            Bot.Logger.LogInformation(
                $"User {user.DisplayName} ({user.Id}) deleted voice channel: {voiceChannel.Name} ({voiceChannel.Id}).");
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.ToString());
        }
    }
}