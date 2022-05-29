namespace BLART.Commands.RoleCommands;

using System.Security.Cryptography;
using System.Text;
using Discord;
using Discord.Interactions;
using MySqlConnector;
using Services;

public partial class RoleCommands
{
    private static SHA256 _sha256 = SHA256.Create();

    private static string GetHashedUserId(string userId)
    {
        byte[] textData = Encoding.UTF8.GetBytes(userId);
        byte[] hash = _sha256.ComputeHash(textData);
        return BitConverter.ToString(hash).Replace("-", string.Empty);
    }

    [SlashCommand("credits", "Syncs your role with the credits database.")]
    public async Task Sync([Summary("userid", "In-game user ID.")] string userId,
        [Summary("role", "The role to assign. (Optional)")] IRole? role = null)
    {
        try
        {
            if (((IGuildUser)Context.User).RoleIds.All(r => r != 656673336402640902 && r != 668651927298375690 && r != 656673780332101648) || BlacklistedDbIds.Contains(Context.User.Id))
            {
                await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied));
                
                return;
            }

            if (!ulong.TryParse(userId, out ulong _))
            {
                await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.UnableToParseId,
                    "You must give a valid ulong for your user ID."));
                return;
            }

            string discordId = GetHashedUserId(Context.User.Id.ToString());
            string hashedUserId = GetHashedUserId(userId);

            if (role is not null && !((IGuildUser) Context.User).RoleIds.Any(r => r == role.Id))
                role = null;

            if (role is null)
            {
                ulong[] roles = ((IGuildUser) Context.User).RoleIds.ToArray();
                if (roles.Any(r => r == 656673336402640902))
                    role = Context.Guild.GetRole(656673336402640902);
                else if (roles.Any(r => r == 668651927298375690))
                    role = Context.Guild.GetRole(668651927298375690);
                else if (roles.Any(r => r == 656673780332101648))
                    role = Context.Guild.GetRole(656673780332101648);
                else
                {
                    await Context.Channel.SendMessageAsync("Congrats, you found an easter egg. (ping joker).");
                    return;
                }
            }

            int rankId = 0;
            switch (role.Id)
            {
                case 656673336402640902:
                    rankId = 1;
                    break;
                case 668651927298375690:
                    rankId = 2;
                    break;
                case 656673780332101648:
                    rankId = 3;
                    break;
            }

            string dbConn =
                $"Server=127.0.0.1;uid={Program.Config.SqlUser};pwd={Program.Config.SqlPassword};database={Program.Config.SqlDatabase};default command timeout=60";
            MySqlConnection conn = new(dbConn);
            conn.Open();
            string readText = "SELECT * FROM `credits`";
            MySqlCommand cmd = new(readText, conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            bool foundMatch = false;
            bool userIdOwned = false;
            while (reader.Read())
            {
                if (reader.GetString("userid") == hashedUserId)
                    userIdOwned = true;
                if (reader.GetString("discordid") == discordId)
                    foundMatch = true;
            }

            reader.Close();

            if (foundMatch)
            {
                cmd = conn.CreateCommand();
                cmd.CommandText = userIdOwned
                    ? $"UPDATE `credits` SET rank=@rankId WHERE discordid=@discId"
                    : "UPDATE `credits` SET rank=@rankId, userid=@userId WHERE discordid=@discId";
                cmd.Parameters.AddWithValue("@rankId", rankId);
                cmd.Parameters.AddWithValue("@userId", hashedUserId);
                cmd.Parameters.AddWithValue("@discId", discordId);
                cmd.Prepare();
                if (cmd.ExecuteNonQuery() > 0)
                {
                    if (userIdOwned)
                        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Rank Updated",
                            $"Your rank has been successfully updated to {role.Name}", Color.Green), ephemeral: true);
                    else
                        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Rank Set",
                            $"Your rank has been set to {role.Name} with the UserID {userId}", Color.Green), ephemeral: true);
                }
                else
                    await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Rank Set ERROR",
                        "Your discord is already registered, but was unable to be updated. Contact Joker.", Color.Red), ephemeral: true);
            }
            else
            {
                cmd = conn.CreateCommand();
                cmd.CommandText =
                    "INSERT INTO `credits`(`id`, `discordid`, `userid`, `rank`) VALUES(@Id, @discordId, @userId, @rankId)";
                cmd.Parameters.AddWithValue("@Id", null);
                cmd.Parameters.AddWithValue("@discordId", discordId);
                cmd.Parameters.AddWithValue("@userId", hashedUserId);
                cmd.Parameters.AddWithValue("@rankId", rankId);
                cmd.ExecuteNonQuery();

                await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Rank Set",
                    $"You have been added to the database with {role.Name} and {userId}.", Color.Green), ephemeral: true);
            }

            conn.Close();
        }
        catch (Exception e)
        {
            Log.Error(nameof(Sync), e);
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.Unspecified, e.Message), ephemeral: true);
        }
    }
}