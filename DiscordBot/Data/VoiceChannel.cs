using DSharpPlus.Entities;
using MySql.Data.MySqlClient;

namespace DiscordBot.Data;

public class ChannelInfo
{
    public ulong ChannelId { get; init; }
    public ulong GuildId { get; set; }
    public ulong UserId { get; init; }
}

public class VoiceChannel
{
    private static readonly Dictionary<string, ChannelInfo> CachedChannelIds = new();

    public async Task<ulong?> GetChannelIdAsync(ulong guildId, ulong userId = 0,
        ChannelType channelType = ChannelType.Usual)
    {
        
        var key = GenerateKey(guildId, userId, channelType);
        if (CachedChannelIds.TryGetValue(key, out var channelInfo))
            return channelInfo.ChannelId;

        MySqlDataReader reader = null;

        try
        {
            var sqlQuery = channelType switch
            {
                ChannelType.Setup => "SELECT * FROM SetupChannels WHERE GuildId = @guildId",
                ChannelType.Usual => "SELECT * FROM VoiceChannels WHERE GuildId = @guildId AND UserId = @userId",
                _ => throw new ArgumentOutOfRangeException(nameof(channelType), channelType, null)
            };

            reader = await Bot.Database.ExecuteQueryAsync(sqlQuery,
                new MySqlParameter("guildId", guildId),
                new MySqlParameter("userId", userId));

            if (!reader.HasRows) return null;
            if (await reader.ReadAsync())
            {
                var channelId = reader.GetUInt64("ChannelId");
                var newChannelInfo = new ChannelInfo
                {
                    ChannelId = channelId,
                    GuildId = guildId,
                    UserId = userId
                };
                CachedChannelIds[key] = newChannelInfo;
                return channelId;
            }
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.ToString());
        }
        finally
        {
            await reader.CloseAsync();
        }

        return null;
    }

    public async Task AddToDatabaseAsync(DiscordChannel voiceChannel, ChannelType channelType = ChannelType.Usual,
        ulong userId = 0)
    {
        try
        {
            var sqlQuery = channelType switch
            {
                ChannelType.Setup => "INSERT INTO SetupChannels (ChannelId, GuildId) " +
                    "VALUES (@channelId, @guildId)",
                ChannelType.Usual => "INSERT INTO VoiceChannels (ChannelId, UserId, GuildId) " +
                    "VALUES (@channelId, @userId, @guildId)",
                _ => throw new ArgumentOutOfRangeException(nameof(channelType), channelType, null)
            };

            await Bot.Database.ExecuteNonQueryAsync(sqlQuery,
                new MySqlParameter("channelId", voiceChannel.Id),
                new MySqlParameter("guildId", voiceChannel.Guild.Id),
                new MySqlParameter("userId", userId));

            var key = GenerateKey(voiceChannel.Guild.Id, userId, channelType);
            var newChannelInfo = new ChannelInfo
            {
                ChannelId = voiceChannel.Id,
                GuildId = voiceChannel.Guild.Id,
                UserId = userId
            };
            CachedChannelIds[key] = newChannelInfo;
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.ToString());
        }
    }

    public async Task DeleteFromDatabaseAsync(DiscordChannel voiceChannel,
        ChannelType channelType = ChannelType.Usual)
    {
        try
        {
            var ownerId = CachedChannelIds.FirstOrDefault(dictionary =>
                dictionary.Value.ChannelId == voiceChannel.Id).Value.UserId;
            var key = GenerateKey(voiceChannel.Guild.Id, ownerId, channelType);
            if (CachedChannelIds.TryGetValue(key, out var channelInfo) && channelInfo.ChannelId == voiceChannel.Id)
                CachedChannelIds.Remove(key);

            var sqlQuery = channelType switch
            {
                ChannelType.Setup => "DELETE FROM SetupChannels WHERE ChannelId = @channelId",
                ChannelType.Usual => "DELETE FROM VoiceChannels WHERE ChannelId = @channelId",
                _ => throw new ArgumentOutOfRangeException(nameof(channelType), channelType, null)
            };

            await Bot.Database.ExecuteNonQueryAsync(sqlQuery,
                new MySqlParameter("channelId", voiceChannel.Id));
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.ToString());
        }
    }

    private static string GenerateKey(ulong guildId, ulong userId, ChannelType channelType) => $"{guildId}_{userId}_{channelType.ToString()}";
}

public enum ChannelType
{
    Setup,
    Usual
}