namespace VoiceMaster.Models;

public class SetupChannel
{
    // Уникальный ID сервера, на котором создан канал для создания каналов
    public ulong GuildId { get; set; }
    
    // Уникальный ID канала создающего канала
    public ulong ChannelId { get; set; }
    
    // Коллекция временных каналов, связанных с этим SetupChannel
    public ICollection<TempChannel>? TempChannels { get; set; }
}