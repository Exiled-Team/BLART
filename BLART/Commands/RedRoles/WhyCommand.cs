namespace BLART.Commands.RedRoles;

using BLART.Objects;
using BLART.Services;
using Discord;
using Discord.Commands;
using Discord.Interactions;

using Group = Discord.Interactions.GroupAttribute;
using Summary = Discord.Interactions.SummaryAttribute;

public partial class RedRoleCommands
{
    [SlashCommand("why", "Tells you why you were given a red role.")]
    public async Task Why()
    {
        PunishmentInfo? info = DatabaseHandler.GetPunishmentInfo(Context.User.Id, DatabaseType.RedRole).FirstOrDefault();
        if (info == null)
        {
            await RespondAsync("You do not have a red role, or it was not given properly.");
            return;
        }

        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Red Role Reason",
            $"You were given your red role on {info.Issued} by {Context.Guild.GetUsername(info.StaffId)} for {info.Reason}.",
            Color.Red));
    }
}