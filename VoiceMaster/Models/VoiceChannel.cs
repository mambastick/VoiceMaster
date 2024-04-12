using Microsoft.EntityFrameworkCore;
using Serilog;
using VoiceMaster.Database;

namespace VoiceMaster.Models
{
    public abstract class VoiceChannel
    {
        // Уникальный ID канала
        public ulong ChannelId { get; set; }

        // Уникальный ID сервера
        public ulong GuildId { get; set; }

        // Абстрактный метод для добавления канала в базу данных
        public abstract Task AddAsync();

        // Абстрактный метод для удаления канала из базы данных
        public abstract Task DeleteAsync();

        // Абстрактный метод для редактирования канала в базе данных
        public abstract Task UpdateAsync();

        // Абстрактный метод для получения канала из базы данных по уникальному ID канала и уникальному ID пользователя
        public abstract Task<VoiceChannel?> GetAsync(ulong channelId, ulong userId);
    }
}