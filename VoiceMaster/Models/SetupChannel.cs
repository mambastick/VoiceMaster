﻿using Microsoft.EntityFrameworkCore;
using Serilog;
using VoiceMaster.Database;

namespace VoiceMaster.Models;

public class SetupChannel : VoiceChannel
{
    // Уникальный ID сервера, на котором создан канал для создания каналов
    public ulong GuildId { get; set; }

    // Уникальный ID канала создающего канала
    public ulong ChannelId { get; set; }

    // Коллекция временных каналов, связанных с этим SetupChannel
    public ICollection<TempChannel>? TempChannels { get; set; }

    public override async Task AddAsync()
    {
        try
        {
            await using var db = new ApplicationContext();
            await db.SetupChannels.AddAsync(this);
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, ex.Message);
        }
    }

    public override async Task DeleteAsync()
    {
        await using var db = new ApplicationContext();
        db.SetupChannels.Remove(this);
        await db.SaveChangesAsync();
    }

    public override async Task UpdateAsync()
    {
        await using var db = new ApplicationContext();
        db.SetupChannels.Update(this);
        await db.SaveChangesAsync();
    }

    public override async Task<VoiceChannel?> GetAsync(ulong guildId, ulong userId)
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
}