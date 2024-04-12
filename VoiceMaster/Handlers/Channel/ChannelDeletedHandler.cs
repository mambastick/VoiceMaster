using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Serilog;
using VoiceMaster.Models;

namespace VoiceMaster.Handlers.Channel;

public class ChannelDeletedHandler
{
    public Task ClientOnChannelDeleted(DiscordClient sender, ChannelDeleteEventArgs args)
    {
        try
        {
            _ = Task.Run(async () =>
            {
                // Если удаленный канал не является голосовым, то ничего не делаем и выходим из функции
                if (args.Channel.Type is not ChannelType.Voice)
                    return;

                // Пользователь, который удалил канал
                DiscordMember? user = null;

                // Так как мы не можем получить свойство User из ChannelDeleteEventArgs
                // нам придется немного поковыряться в журнале аудиата Discord сервера
                
                // Получаем аудит-логи для сервера
                var auditLogs = await args.Guild.GetAuditLogsAsync(limit: 1);

                // Получаем последнюю запись из аудит-логов
                var lastLog = auditLogs.FirstOrDefault();

                // Проверяем, был ли это лог об удалении канала и если да, кто его удалил
                if (lastLog is { ActionType: AuditLogActionType.ChannelDelete })
                {
                    // Пользователь, который удалил канал
                    var userWhoDeletedChannel = lastLog.UserResponsible;

                    // Если это бот, то выходим и ничего не делаем
                    if (userWhoDeletedChannel.IsBot)
                        return;

                    user = userWhoDeletedChannel as DiscordMember;
                }

                // Сервер, где удалили канал
                var guild = args.Guild;

                // Удаленный голосовой канал
                var deletedChannel = args.Channel;

                // Если создающий канал существует и его ID схож с ID удаленного канала
                if (await new SetupChannel().GetAsync(guild.Id, deletedChannel.Id) is SetupChannel setupChannel &&
                    setupChannel.ChannelId == deletedChannel.Id)
                {
                    // Удаляем создающий канал
                    await setupChannel.DeleteAsync();
                    return;
                }
                
                // Если временный канал существует и его ID схож с ID удаленного канала
                if (await new TempChannel().GetAsync(deletedChannel.Id, user.Id) is TempChannel
                        tempChannel && tempChannel.ChannelId == deletedChannel.Id)
                {
                    // Удаляем создающий канал
                    await tempChannel.DeleteAsync();
                }
            });
        }
        catch (Exception ex)
        {
            Log.Logger.Error(ex, ex.Message);
            throw;
        }

        return Task.CompletedTask;
    }
}