namespace BLART.Modules;

using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;

public class SpamPrevention
{
    private static Dictionary<SocketUser, (DateTime, int)> SpamTracker { get; } = new();
    private static readonly Regex Regex = new(@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static async Task OnMessageReceived(SocketMessage message)
    {
        if (Check(message.Author) || Check(message))
        {
            await Logging.SendLogMessage($"User auto-{(RaidProtection.Check(message.Author) ? "banned" : "muted")}", $"{message.Author.Username} has been auto-moderated for spamming.", Color.Red);
            
            if (RaidProtection.Check(message.Author))
                await ((IGuildUser)message.Author).BanAsync(7, "Raid protection triggered (spamming)");
            else
            {
                await ((IGuildUser)message.Author).SetTimeOutAsync(TimeSpan.FromHours(2));
                await message.DeleteAsync();
                int count = 0;
                foreach (SocketTextChannel channel in Bot.Instance.Guild.TextChannels)
                {
                    foreach (IMessage msg in await channel.GetMessagesAsync(20).FlattenAsync())
                    {
                        if (msg.Author.Id == message.Author.Id && (DateTime.UtcNow - msg.Timestamp).TotalMinutes < 5)
                        {
                            await msg.DeleteAsync();
                            count++;

                            if (count > 20)
                                break;
                        }
                    }

                    if (count > 20)
                        break;
                }
            }
        }
    }

    private static bool Check(SocketUser user)
    {
        if (user.IsBot)
            return false;
        
        if (!SpamTracker.ContainsKey(user))
            SpamTracker.Add(user, new(DateTime.UtcNow, 0));

        if ((DateTime.UtcNow - SpamTracker[user].Item1).TotalMinutes > 1)
        {
            SpamTracker[user] = new(DateTime.UtcNow, 1);
            return false;
        }

        (DateTime time, int count) = SpamTracker[user];
        SpamTracker[user] = new(time, count + 1);

        return SpamTracker[user].Item2 > Program.Config.SpamLimit;
    }

    private static bool Check(SocketMessage message) => FrequencyCount("<@", message.Content) > 4 || message.Content.Split(' ').Any(s => Regex.IsMatch(s) && IsBlockedContent(s));

    private static bool IsBlockedContent(string url)
    {
        url = url.ToLowerInvariant();
        
        if (url.Contains("pornhub.com") || url.Contains("xvideos.com") || url.Contains("hentaihaven.com") ||
            url.Contains("redtube.com") || url.Contains("bestgore.com"))
            return true;
        
        if (url.Contains("discord.gg"))
            if (!url.Contains("scpsl") && !url.Contains("PyUkWTg"))
                return true;
        
        return false;
    }

    private static int FrequencyCount(string pattern, string text)
    {
        int m = pattern.Length;
        int n = text.Length;
        int result = 0;

        for (int i = 0; i <= n - m; i++)
        {
            int j;
            for (j = 0; j < m; j++)
                if (text[i + j] != pattern[j])
                    break;

            if (j != m) 
                continue;
            result++;
        }

        return result;
    }
}