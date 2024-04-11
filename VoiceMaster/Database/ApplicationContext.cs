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
                               $"password={env["DATABASE_PASSWORD"]}";
        
        // Используем базу данных MySQL
        optionsBuilder.UseMySQL(
            connectionString: connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SetupChannel>()
            .HasKey(sc => sc.ChannelId);

        modelBuilder.Entity<TempChannel>()
            .HasKey(tc => tc.ChannelId);

        modelBuilder.Entity<TempChannel>()
            .HasOne(sc => sc.SetupChannel)
            .WithMany(s => s.TempChannels)
            .HasForeignKey(sc => sc.SetupChannelId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}