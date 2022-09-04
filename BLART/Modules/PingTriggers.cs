namespace BLART.Modules;

using System.Collections.Concurrent;
using BLART.Services;
using Discord;
using Discord.WebSocket;

public class PingTriggers
{
    private static ConcurrentDictionary<SocketUser, DateTime> _lastPing { get; } = new();

    public static async Task HandleMessage(SocketMessage msg)
    {
        if (msg.Author.IsBot)
            return;

        try
        {
            if (_lastPing.ContainsKey(msg.Author) && (DateTime.UtcNow - _lastPing[msg.Author]).TotalMinutes < 2)
            {
                Log.Debug($"{nameof(Log)}.{nameof(HandleMessage)}",
                    $"Last ping too recent, returning. {_lastPing[msg.Author]} {(DateTime.UtcNow - _lastPing[msg.Author]).TotalMinutes}");
                return;
            }
            
            foreach (SocketUser mentioned in msg.MentionedUsers)
            {
                string triggerMessage = DatabaseHandler.GetPingTrigger(mentioned.Id);
                if (!string.IsNullOrEmpty(triggerMessage) && triggerMessage.Length < Program.Config.TriggerLengthLimit)
                {
                    await msg.Channel.SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Ping Trigger", $"{msg.Author.Mention} - {triggerMessage}", Color.Gold));
                    _lastPing[msg.Author] = DateTime.UtcNow;
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