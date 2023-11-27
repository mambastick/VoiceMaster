using DiscordBot.Data;
using DSharpPlus;
using DSharpPlus.EventArgs;
using ChannelType = DSharpPlus.ChannelType;

namespace DiscordBot.Handlers;

public class Channel
{
    public static Task ChannelDeletedHandlerAsync(DiscordClient client, ChannelDeleteEventArgs deletedChannel)
    {
        try
        {
            _ = Task.Run(async () =>
            {
                if (deletedChannel.Channel.Type is not ChannelType.Voice)
                    return;

                var databaseChannel = new VoiceChannel();
                var databaseChannelId = await databaseChannel.GetChannelIdAsync(deletedChannel.Guild.Id);
                if (deletedChannel.Channel.Id == databaseChannelId)
                {
                    await databaseChannel.DeleteFromDatabaseAsync(deletedChannel.Channel);
                    return;
                }

                databaseChannelId =
                    await databaseChannel.GetChannelIdAsync(deletedChannel.Guild.Id,
                        channelType: Data.ChannelType.Setup);

                if (deletedChannel.Channel.Id == databaseChannelId)
                    await databaseChannel.DeleteFromDatabaseAsync(deletedChannel.Channel, Data.ChannelType.Setup);
            });
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.ToString());
        }
        
        return Task.CompletedTask;
    }
}