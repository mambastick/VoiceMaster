using System.Collections;

namespace VoiceMaster;

public abstract class AppSettings
{
    public static IDictionary GetAppSettings()
    {
        var env = new Dictionary<string, string>();

        // Проверяем наличие файла .env и считываем переменные, если файл существует
        if (File.Exists(".env"))
        {
            var lines = File.ReadAllLines(".env");
            foreach (var line in lines)
            {
                if (line.StartsWith('#'))
                    continue;
                    
                var parts = line.Split('=', 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();
                    env[key] = value;
                }
            }
        }
        else // Если файла .env нет, читаем переменные из среды системы
        {
            var environmentVariables = Environment.GetEnvironmentVariables();
            foreach (var key in environmentVariables.Keys)
            {
                env[key.ToString()] = environmentVariables[key].ToString();
            }
        }
        
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