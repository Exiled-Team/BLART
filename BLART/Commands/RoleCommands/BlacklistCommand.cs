namespace BLART.Commands.RoleCommands;

using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Services;

public partial class RoleCommands
{
    public List<ulong> BlacklistedDbIds { get; } = new();
    
    private void SaveBlacklist(string s)
    {
        if (!File.Exists("blacklist.txt"))
            File.Create("blacklist.txt").Close();
        File.AppendAllText("blacklist.txt", s + Environment.NewLine);

        ReadBlacklist();
    }

    private void ReadBlacklist()
    {
        if (!File.Exists("blacklist.txt"))
            File.Create("blacklist.txt").Close();

        BlacklistedDbIds.Clear();

        foreach (string s in File.ReadAllLines("blacklist.txt"))
            if (ulong.TryParse(s, out ulong id))
                BlacklistedDbIds.Add(id);
    }

    [SlashCommand("blacklist", "Stops a user from using the sync command.")]
    public async Task Blacklist([Summary("user", "The user to blacklist.")] SocketUser user)
    {
        if (Context.User.Id != 373626508964659201)
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied));
            
            return;
        }

        SaveBlacklist(user.Id.ToString());
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Blacklist Added",
            $"{user.Username} has been bonked.", Color.Green));
    }
}