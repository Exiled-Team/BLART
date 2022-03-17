namespace BLART.Commands.RedRoles;

using BLART.Objects;
using BLART.Services;
using Discord;
using Discord.Commands;

public class WhyCommand : ModuleBase<SocketCommandContext>
{
    [Command("why")]
    [Summary("Tells you why you have a red role.")]
    public async Task Why()
    {
        PunishmentInfo? info = DatabaseHandler.GetPunishmentInfo(Context.Message.Author.Id, DatabaseType.RedRole).FirstOrDefault();
        if (info == null)
        {
            await ReplyAsync("You do not have a red role, or it was not given properly.");
            return;
        }

        await ReplyAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Red Role Reason",
            $"You were given your red role on {info.Issued} by {Context.Guild.GetUsername(info.StaffId)} for {info.Reason}.",
            Color.Red));
    }
}