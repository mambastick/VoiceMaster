using System.Collections;

namespace VoiceMaster;

public abstract class AppSettings
{
    public static IDictionary GetAppSettings()
    {
        // Получаем все переменные среды
        var env = Environment.GetEnvironmentVariables();
        
        // Проверка переменных сред
        ValidateEnvironment(env);

        return env;
    }

    private static void ValidateEnvironment(IDictionary env)
    {
        // Проверяем наличие ожидаемых переменных среды
        var expectedVariables = new List<string> { "BOT_TOKEN", "WEBHOOK_ID", "WEBHOOK_URL", "DATABASE_HOST", "DATABASE_PORT", "DATABASE_NAME", "DATABASE_USER", "DATABASE_PASSWORD" };
        foreach (var key in expectedVariables.Where(key => !env.Contains(key)))
            throw new Exception($"Отсутствует переменная среды {key}!");
    }
}