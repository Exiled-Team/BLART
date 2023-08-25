namespace BLART.Modules;

using BLART.Services;
using Discord;
using Discord.WebSocket;

public class ChannelRenting
{
     public static Dictionary<SocketUser, ulong> RentedChannels { get; } = new();

        private static List<string> ChannelNames { get; } = new()
        {
            "A spooky %user's channel",
            "%user's gulag",
            "%user's lair",
            "%user's den",
            "Castle de %user",
            "%user's a nerd",
            "%user's Playground",
        };

        public static async Task OnVoiceStateChanged(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            if (after.VoiceChannel != null && after.VoiceChannel.Id == Program.Config.ChannelRentId)
                await HandleJoined(user, before, after);
            else if (before.VoiceChannel != null && IsRented(before.VoiceChannel.Id))
                await HandleLeft(user, before, after);
        }

        private static async Task HandleJoined(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            IGuildUser guildUser = (IGuildUser)user;
            if (RentedChannels.ContainsKey(user))
            {
                Log.Debug($"{nameof(ChannelRenting)}.{nameof(HandleJoined)}","User already has a rented channel.");
                return;
            }
            
            if (after.VoiceChannel.Id != Program.Config.ChannelRentId)
            {
                Log.Error($"{nameof(ChannelRenting)}.{nameof(HandleJoined)}","User joined a non-rent channel.");
                return;
            }

            int r = Program.Rng.Next(ChannelNames.Count);
            string userName = guildUser.Nickname != null && !string.IsNullOrEmpty(guildUser.Nickname)
                ? guildUser.Nickname
                : guildUser.Username;
            string chanName = ChannelNames[r].Replace("%user", userName);

            IGuild guild = after.VoiceChannel.Guild;
            if (guild == null)
            {
                Log.Error($"{nameof(ChannelRenting)}.{nameof(HandleJoined)}", "Guild is null ?!?!?");
                return;
            }

            var channel = await guild.CreateVoiceChannelAsync(chanName, properties =>
            {
                properties.UserLimit = 10;
                properties.Name = chanName;
                properties.CategoryId = Program.Config.ChannelRentCatId;
            }, RequestOptions.Default);

            await channel.AddPermissionOverwriteAsync(guild.EveryoneRole,
                new OverwritePermissions(connect: PermValue.Deny));
            await channel.AddPermissionOverwriteAsync(guildUser, new(connect: PermValue.Allow, stream: PermValue.Allow));
            var staffRole = guild.GetRole(Program.Config.DiscStaffId);
            await channel.AddPermissionOverwriteAsync(staffRole, new OverwritePermissions(connect: PermValue.Allow, manageChannel: PermValue.Allow));

            await guildUser.ModifyAsync(x => x.ChannelId = channel.Id);
            
            RentedChannels.Add(user, channel.Id);
        }

        private static async Task HandleLeft(SocketUser user, SocketVoiceState before, SocketVoiceState after)
        {
            if (before.VoiceChannel.Users.Count == 0)
            {
                await before.VoiceChannel.DeleteAsync();
                RentedChannels.Remove(user);
            }
        }

        public static bool IsRented(ulong channel) => RentedChannels.ContainsValue(channel);
        public static bool IsRenting(SocketUser user) => RentedChannels.ContainsKey(user);
}