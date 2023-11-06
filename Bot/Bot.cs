using DSharpPlus;

namespace Bot;

public class Bot(string token)
{
    private string Token { get; set; } = token;

    public async Task Start()
    {
        var discord = new DiscordClient(new DiscordConfiguration
        {
            Token = Token,
            TokenType = TokenType.Bot
        });
        
        
    }
}