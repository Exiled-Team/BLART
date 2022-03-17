namespace BLART.Commands.ChannelRenting;

using System.Threading.Tasks;
using BLART.Modules;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class PermitCommand : ModuleBase<SocketCommandContext>
{
    [Command("rent allow")]
    [Summary("Permits a user to join your rented channel. (Only usable while renting a channel)")]
    public async Task Permit([Summary("The user(s) to permit")] [Remainder] string users)
    {
        if (!ChannelRenting.IsRenting(Context.Message.Author))
        {
            await ReplyAsync("You must be renting a channel to use this command.");
            return;
        }

        IVoiceChannel channel = Bot.Instance.Guild.GetVoiceChannel(ChannelRenting.RentedChannels[Context.Message.Author]);
            
        foreach (SocketUser user in Context.Message.MentionedUsers)
        {
            await channel.AddPermissionOverwriteAsync(user, new OverwritePermissions(connect: PermValue.Allow));
        }

        await ReplyAsync("The users mentioned have been permitted to connect to your channel.");
    }
}