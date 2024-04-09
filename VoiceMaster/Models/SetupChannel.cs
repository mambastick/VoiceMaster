using Microsoft.EntityFrameworkCore;
using Serilog;
using VoiceMaster.Database;

namespace VoiceMaster.Models;

public class SetupChannel()
{
    // Уникальный ID сервера, на котором создан канал для создания каналов
    public ulong GuildId { get; set; }

    // Уникальный ID канала создающего канала
    public ulong ChannelId { get; set; }

    // Коллекция временных каналов, связанных с этим SetupChannel
    public ICollection<TempChannel>? TempChannels { get; set; }

    public async Task AddAsync()
    {
        try
        {
            await using var db = new ApplicationContext();
            await db.AddAsync(this);
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, ex.Message);
        }
    }

    public async Task<SetupChannel?> GetAsync(ulong guildId)
    {
        try
        {
            await using var db = new ApplicationContext();
            return await db.SetupChannels
                .Include(tc => tc.TempChannels)
                .FirstOrDefaultAsync(sc => sc.GuildId == guildId);
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, ex.Message);
        }

        return null;
    }

    public async Task DeleteAsync()
    {
        try
        {
            await using var db = new ApplicationContext();
            db.SetupChannels.Remove(this);
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, ex.Message);
        }
    }
}