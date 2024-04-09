using Microsoft.EntityFrameworkCore;
using Serilog;
using VoiceMaster.Database;

namespace VoiceMaster.Models
{
    public class TempChannel
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

        public async Task<TempChannel?> GetAsync(ulong guildId, ulong userId)
        {
            try
            {
                await using var db = new ApplicationContext();
                return await db.TempChannels
                    .Include(sc => sc.SetupChannel)
                    .FirstOrDefaultAsync(tc => tc.GuildId == guildId && tc.UserId == userId);
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
                db.TempChannels.Remove(this);
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, ex.Message);
            }
        }
    }
}