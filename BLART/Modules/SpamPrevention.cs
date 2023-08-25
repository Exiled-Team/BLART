namespace BLART.Modules;

using System.Text;
using System.Text.RegularExpressions;
using Commands;
using Discord;
using Discord.WebSocket;

public class SpamPrevention
{
    private static Dictionary<SocketUser, (DateTime, int)> SpamTracker { get; } = new();
    private static readonly Regex Regex = new(@"^(?:http(s)?:\/\/)?[\w.-]+(?:\.[\w\.-]+)+[\w\-\._~:/?#[\]@!\$&'\(\)\*\+,;=.]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static List<string> BlacklistWebSite = new()
    {
    "xnxx.com",
    "pornhub.com",
    "xvideos.com",
    "fuq.com",
    "xhamster.com",
    "tukif.com",
    "youporn.com",
    "mvideoporno.com",
    "qorno.com",
    "porn300.com",
    "ixxx.com",
    "youporngay.com",
    "fapvid.com",
    "pornoplus.com",
    "vidmo.com",
    "absoluporn.com",
    "its.porn",
    "bestgore.com",
    "hentaihaven.com"
    };
    private static List<string> WhitelistedDiscord = new()
    {
    "scpsl", // scpsl official
    "PyUkWTg", // Exiled official
    "dtPGCsm", // DiscoHook official
    "csharp", // Csharp official
    "p69SGfwxxm", // CedMod by ced777ric
    "3j54zBnbbD", // Scripted-Event by Thunder's
    };
    public static Task OnMessageReceived(SocketMessage arg) => OnMessageReceived(arg, false);

    public static async Task OnMessageReceived(SocketMessage message, bool skipSpam)
    {
        bool spam = Check(message.Author, skipSpam);
        bool link = Check(message);
        
        if (spam || link)
        {
            await Logging.SendLogMessage($"User auto-{(RaidProtection.Check(message.Author) ? "banned" : "muted")}", $"{message.Author.Username} has been auto-moderated for {(spam ? "spamming" : "linking")}.", Color.Red);
            
            if (RaidProtection.Check(message.Author))
                await ((IGuildUser)message.Author).BanAsync(7, "Raid protection triggered (spamming/linking)");
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

    public static async Task OnMessageUpdated(Cacheable<IMessage, ulong> orig, SocketMessage edited, ISocketMessageChannel channel) => await OnMessageReceived(edited);

    private static bool Check(SocketUser user, bool skipSpam, bool interaction = false)
    {
        if (user.IsBot || CommandHandler.CanRunStaffCmd(user) || skipSpam)
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

        return SpamTracker[user].Item2 > (interaction ? Program.Config.SpamLimit / 2 : Program.Config.SpamLimit);
    }

    private static bool Check(SocketMessage message)
    {
        if (CommandHandler.CanRunStaffCmd(message.Author))
            return false;
        if (FrequencyCount("<@", message.Content) > 4)
            return true;
        if (message.Content.Replace(" ", string.Empty).Contains("discordgg") ||
            message.Content.Split(' ').Any(s => Regex.IsMatch(s)))
        {
            foreach (string s in message.Content.Split(' '))
            {
                string decoded = string.Empty;
                try
                {
                    decoded = Encoding.UTF8.GetString(Convert.FromBase64String(s));
                }
                catch (Exception)
                {
                    // ignored
                }

                if (IsBlockedContent(s) || (!string.IsNullOrEmpty(decoded) && IsBlockedContent(decoded)))
                    return true;
            }
        }

        return false;
    }

    private static bool IsBlockedContent(string url)
    {
        url = url.ToLowerInvariant();
        
        if (BlacklistWebSite.Any(x => BlacklistWebSite.Contains(x)))
            return true;
        
        if (url.Contains("discord.gg"))
            if (WhitelistedDiscord.Any(x => url.EndsWith(x)))
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

    public static async Task HandleInteraction(SocketInteraction message)
    {
        if (Check(message.User, false, true))
        {
            await Logging.SendLogMessage($"User auto-{(RaidProtection.Check(message.User) ? "banned" : "muted")}",
                $"{message.User.Username} has been auto-moderated for spamming.", Color.Red);

            if (RaidProtection.Check(message.User))
                await ((IGuildUser) message.User).BanAsync(7, "Raid protection triggered (spamming/linking)");
            else
            {
                await ((IGuildUser) message.User).SetTimeOutAsync(TimeSpan.FromHours(6));
                int count = 0;
                foreach (SocketTextChannel channel in Bot.Instance.Guild.TextChannels)
                {
                    foreach (IMessage msg in await channel.GetMessagesAsync(20).FlattenAsync())
                    {
                        if (msg.Author.Id == message.User.Id && (DateTime.UtcNow - msg.Timestamp).TotalMinutes < 5)
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
}