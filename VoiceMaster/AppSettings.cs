using dotenv.net;

namespace VoiceMaster;

public abstract class AppSettings
{
    public static IDictionary<string, string> GetAppSettings()
    {
        // Загрузка переменных сред
        var env = DotEnv.Read();
        
        // Проверка переменных сред
        ValidateEnvironment(env);

        return env;
    }

    private static void ValidateEnvironment(IDictionary<string, string> env)
    {
        foreach (var key in env.Keys)
        {
            var value = env[key];
            if (string.IsNullOrEmpty(value))
            {
                throw new Exception($"{key} не может быть пустым!");
            }
        }
    }
}