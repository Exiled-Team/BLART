namespace BLART.Commands.ChannelRenting;

using System.Threading.Tasks;
using BLART.Modules;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

public class DenyCommand : ModuleBase<SocketCommandContext>
{
    [Command("rent deny")]
    [Summary("Denies user(s) from your channel.")]
    public async Task Deny([Summary("The users to deny access.")] [Remainder] string users)
    {
        if (!ChannelRenting.IsRenting(Context.Message.Author))
        {
            await ReplyAsync("You can only use this command while renting a channel.");
            return;
        }

        IVoiceChannel channel = Bot.Instance.Guild.GetVoiceChannel(ChannelRenting.RentedChannels[Context.Message.Author]);
        foreach (SocketUser user in Context.Message.MentionedUsers)
            await channel.AddPermissionOverwriteAsync(user, new OverwritePermissions(connect: PermValue.Deny));

        await ReplyAsync("The mentioned users have been denied access to your channel.");
    }
}