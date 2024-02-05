namespace BLART.Modules;

using System.Collections.Concurrent;
using BLART.Services;
using Discord;
using Discord.WebSocket;

public class PingTriggers
{
    private static ConcurrentDictionary<SocketUser, DateTime> LastPing { get; } = new();

    public static async Task HandleMessage(SocketMessage msg)
    {
        if (msg.Author.IsBot)
            return;

        try
        {
            if (LastPing.TryGetValue(msg.Author, out DateTime value) && (DateTime.UtcNow - value).TotalMinutes < 2)
                return;

            foreach (SocketUser mentioned in msg.MentionedUsers)
            {
                string triggerMessage = DatabaseHandler.GetPingTrigger(mentioned.Id);
                if (!string.IsNullOrEmpty(triggerMessage) && triggerMessage.Length < Program.Config.TriggerLengthLimit)
                {
                    await msg.Channel.SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Ping Trigger", $"{msg.Author.Mention} - {triggerMessage}", Color.Gold));
                    LastPing[msg.Author] = DateTime.UtcNow;
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Log.Error(nameof(HandleMessage), e);
        }
    }
}