

using Database;
using DiscordBot;
using LoggerService;

namespace VoiceMaster;

internal abstract class Program
{
    private static async Task Main()
    {
        var bot = new Bot(
            "MTE3MDg3MTc1MjU2MjMxNTI2NQ.GQ4Fgm.a7wz7WzPsWOqVpY9IQXhug3mJ-pZFscB3DKNZc",
            new Logger(),
            new MySqlDatabase("mambaskills.ru", "VoiceMaster", "VoiceMaster", "jVX-6ti-m3V-yYq")); 
        await bot.StartAsync();
        await Task.Delay(-1);
    }
}