using Microsoft.EntityFrameworkCore;
using Serilog;
using VoiceMaster.Database;

namespace VoiceMaster.Models
{
    public class TempChannel : VoiceChannel
    {
        // Уникальный ID созданного канала
        public ulong ChannelId { get; set; }

        // ID пользователя, которым был создан канал
        public ulong UserId { get; set; }

        // ID сервера, на котором создан канал
        public ulong GuildId { get; set; }

        // ID связанного SetupChannel
        public ulong SetupChannelId { get; set; }

        // Навигационное свойство к SetupChannel
        public SetupChannel SetupChannel { get; set; }

        public override async Task AddAsync()
        {
            try
            {
                await using var db = new ApplicationContext();
                await db.TempChannels.AddAsync(this);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, ex.Message);
            }
        }

        public override async Task DeleteAsync()
        {
            try
            {
                await using var db = new ApplicationContext();
                db.TempChannels.Remove(this);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, ex.Message);
            }
        }

        public override async Task UpdateAsync()
        {
            try
            {
                await using var db = new ApplicationContext();
                db.TempChannels.Update(this);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, ex.Message);
            }
        }

        public override async Task<VoiceChannel?> GetAsync(ulong guildId, ulong userId)
        {
            try
            {
                await using var db = new ApplicationContext();
                return await db.TempChannels
                    .Include(sc => sc.SetupChannel)
                    .FirstOrDefaultAsync(tc =>
                    tc.GuildId == guildId && tc.UserId == userId);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, ex.Message);
            }

            return null;
        }
    }
}