using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Serilog;
using VoiceMaster.Models;

namespace VoiceMaster.Handlers;

public class VoiceStateUpdateHandler
{
    public Task ClientOnVoiceStateUpdated(DiscordClient sender, VoiceStateUpdateEventArgs e)
    {
        try
        {
            // Создаем новый поток, когда пользователь входит или выход из голосового канала
            Task.Run(async () =>
            {
                // Получаем данные о создающем голосовом канале из базы данных
                var setupVoiceChannel = await new SetupChannel().GetAsync(e.Guild.Id);

                // Если создающий голосовой канал не найден или не существует, ничего дальше не делаем
                if (setupVoiceChannel is null)
                    return;

                // Если создается канал
                if (
                    e.After.Channel != null &&
                    e.After.Channel.Id == setupVoiceChannel.ChannelId
                )
                    await new VoiceStateUpdateHandler().CreateVoiceChannelAsync(e);
                // Если удаляется канал
                else if (
                    e.Before?.Channel != null &&
                    e.Before.Channel.Id != setupVoiceChannel.ChannelId &&
                    e.Before.Channel.Users.Count == 0
                )
                    await new VoiceStateUpdateHandler().DeleteVoiceChannelAsync(e);
            });
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, ex.Message);
        }

        return Task.CompletedTask;
    }

    private async Task CreateVoiceChannelAsync(VoiceStateUpdateEventArgs e)
    {
        try
        {
            // Пользователь
            var user = e.User as DiscordMember;

            // Проверяем: есть ли временный голосовой канал у пользователя на данном сервере ?
            var dbTempVoiceChannel = await new TempChannel().GetAsync(e.Guild.Id, user.Id);

            // Если временный голосовой канал у пользователя создан
            if (dbTempVoiceChannel is not null)
            {
                // Получаем данные об этом временном канале
                var tempVoiceChannel = e.Guild.GetChannel(dbTempVoiceChannel.ChannelId);

                // Перемещаем пользователя в его временный канал
                await user.ModifyAsync(properties =>
                    properties.VoiceChannel = tempVoiceChannel);

                Log.Logger.Information(
                    $"Пользователь {user.DisplayName} ({user.Id}) перемещен в голосовой канал " +
                    $"{user.VoiceState?.Channel?.Name} ({user.VoiceState?.Channel?.Id}) " +
                    $"(Попытка создать второй канал).");
            }
            // Иначе, если же временный голосовой канал у пользователя не создан
            else
            {
                // Создаем новый временный канал для пользователя
                var newTempVoiceChannel = await e.Guild.CreateChannelAsync(
                    $"Канал - {user?.DisplayName}",
                    ChannelType.Voice,
                    e.Channel.Parent,
                    reason: $"Пользователь {user.DisplayName} ({user.Id}) создал новый временный голосовой канал");

                // Перемещаем пользователя в его новый временный канал
                await user.ModifyAsync(properties => properties.VoiceChannel = newTempVoiceChannel);
                
                // TODO: Исправить ошибку с добавлением в базу данных
                // Добавляем временный голосовой канал в базу данных
                await new TempChannel
                {
                    ChannelId = newTempVoiceChannel.Id,
                    GuildId = newTempVoiceChannel.Guild.Id,
                    UserId = user.Id,
                    SetupChannelId = e.Channel.Id,
                }.AddAsync();

                Log.Logger.Information(
                    $"Пользователь {user.DisplayName} ({user.Id}) создал временный голосовой канал: " +
                    $"{user.VoiceState?.Channel?.Name} ({user.VoiceState?.Channel?.Id}).");
            }
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, ex.Message);
        }
    }

    private async Task DeleteVoiceChannelAsync(VoiceStateUpdateEventArgs e)
    {
        try
        {
            // Временный голосовой канал
            var tempVoiceChannel = e.Before.Channel;
            
            // Пользователь
            var user = e.User as DiscordMember;
            
            // Удаляем временный голосовой канал
            await tempVoiceChannel.DeleteAsync($"Пользователь {user.DisplayName} удалил голосовой канал");

            // Удаляем информацию о временном голосовом канале из базы данных
            await new TempChannel()
                {
                    ChannelId = tempVoiceChannel.Id,
                    GuildId = tempVoiceChannel.Guild.Id
                }
                .DeleteAsync();
            
            Log.Logger.Information(
                $"Пользователь {user.DisplayName} ({user.Id}) удалил голосовой канал: {tempVoiceChannel.Name} ({tempVoiceChannel.Id}).");
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, ex.Message);
        }
    }
}