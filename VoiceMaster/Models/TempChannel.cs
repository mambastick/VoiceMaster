using System;

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
    }
}