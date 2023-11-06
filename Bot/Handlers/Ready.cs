﻿using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Bot.Handlers;

public static class Ready
{
    public static Task ClientReady(DiscordClient sender, ReadyEventArgs e)
    {
        Bot.Logger.LogSuccess("Бот готов к получению событий.");
        return Task.CompletedTask;
    }
}