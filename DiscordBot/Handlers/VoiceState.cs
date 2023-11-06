using System.Text.Json;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace DiscordBot.Handlers;

public class VoiceState
{
    private static readonly string CreatedVoiceChannelName = "🔊│Создать канал";
    public static Dictionary<ulong, ulong> VoiceChannels = new();

    private async Task CreateVoiceChannelAsync(DiscordClient sender, VoiceStateUpdateEventArgs e)
    {
        
    }

    private async Task DeleteVoiceChannelAsync(DiscordClient sender, VoiceStateUpdateEventArgs e)
    {
        
    }

    public static async Task VoiceStateUpdatedAsync(DiscordClient sender, VoiceStateUpdateEventArgs e)
    {
        try
        {
            Bot.Logger.LogInformation(JsonSerializer.Serialize(e));
        }
        catch (Exception ex)
        {
            Bot.Logger.LogError(ex.Message);
        }
    }
}