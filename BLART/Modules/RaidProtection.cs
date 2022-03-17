namespace BLART.Modules;

using System.Collections.Concurrent;
using Discord;
using Discord.WebSocket;

public class RaidProtection
{
    private static ConcurrentBag<SocketGuildUser> Users { get; } = new();
    
    public static bool Active { get; set; }

    public static Task OnUserJoined(SocketGuildUser user)
    {
        if (Active)
            Users.Add(user);
        return Task.CompletedTask;
    }

    public static bool Check(IUser user) => Active && Check((SocketGuildUser)user);
    public static bool Check(SocketUser user) => Active && Check((SocketGuildUser)user);
    public static bool Check(SocketGuildUser user) => Active && Users.Contains(user);

    public static void ClearUsers() => Users.Clear();
}