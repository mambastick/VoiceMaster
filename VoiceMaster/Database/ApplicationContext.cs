using Microsoft.EntityFrameworkCore;
using VoiceMaster.Models;

namespace VoiceMaster.Database;

public class ApplicationContext : DbContext
{
    // Создание таблиц
    public DbSet<SetupChannel> SetupChannels => Set<SetupChannel>(); // Создающие голосовые каналы
    public DbSet<TempChannel> TempChannels => Set<TempChannel>(); // Создаваемые голосовые каналы

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Загрузка переменных сред
        var env = AppSettings.GetAppSettings();
        
        // Строка подключения к базе данных
        var connectionString = $"server={env["DATABASE_HOST"]};" +
                               $"port={env["DATABASE_PORT"]};" +
                               $"database={env["DATABASE_NAME"]};" +
                               $"user={env["DATABASE_USER"]};" +
                               $"password={env["DATABASE_PASSWORD"]}";;
        
        // Вычисляем версию MySQL
        var mySqlVersion = ServerVersion.AutoDetect(connectionString);
        
        // Используем базу данных MySQL
        optionsBuilder.UseMySql(
            connectionString: connectionString,
            serverVersion: mySqlVersion
            );
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройка первичных ключей и уникальных ограничений
        modelBuilder.Entity<SetupChannel>()
            .HasKey(s => s.GuildId); // Уникальный ID сервера как первичный ключ
        modelBuilder.Entity<TempChannel>()
            .HasKey(t => t.ChannelId); // Уникальный ID канала как первичный ключ
        modelBuilder.Entity<TempChannel>()
            .HasIndex(t => t.UserId); // Индекс для ускорения поиска по UserId

        // Настройка связи между таблицами с каскадным удалением
        modelBuilder.Entity<SetupChannel>()
            .HasMany(s => s.TempChannels)
            .WithOne(t => t.SetupChannel)
            .HasForeignKey(t => t.SetupChannelId) // Используем SetupChannelId как внешний ключ
            .OnDelete(DeleteBehavior.Cascade);
    }
}