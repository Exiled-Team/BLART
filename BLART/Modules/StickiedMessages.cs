using BLART.Objects;
using BLART.Services;
using Discord;
using Discord.WebSocket;
using System;

namespace BLART.Modules;

public class StickiedMessages
{
    public static async Task Post(StickyMessage message)
    {
        IGuild guild = Bot.Instance.Guild;

        IGuildUser staff = await guild.GetUserAsync(message.StaffId);
        staff ??= await guild.GetCurrentUserAsync();

        ITextChannel textChannel = await guild.GetTextChannelAsync(message.ChannelId);
        if (textChannel != null)
        {
            IUserMessage m = await textChannel.SendMessageAsync(embed: await EmbedBuilderService.CreateStickyMessage(message.Message, staff));
            DatabaseHandler.AddEntry(m.Id, textChannel.Id.ToString(), DatabaseType.StickiedMessageIDs);
        }
    }

    public static async Task OnMessageReceived(SocketMessage message)
    {
        StickyMessage? msg = DatabaseHandler.GetStickyMessage(message.Channel.Id);
        if (msg is null)
            return;

        string? stickyId = DatabaseHandler.GetStickyMessageID(msg.ChannelId);

        if (stickyId is not null)
        {
            IMessage current = await message.Channel.GetMessageAsync(ulong.Parse(stickyId));
            if ((DateTime.UtcNow - current.Timestamp).TotalSeconds < 5)
                return;

            await current.DeleteAsync(new() { AuditLogReason = "Creating new sticky message." });
            DatabaseHandler.RemoveEntry(current.Id, DatabaseType.StickiedMessageIDs);
        }

        await Post(msg);
    }
}