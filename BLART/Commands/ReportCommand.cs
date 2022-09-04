namespace BLART.Commands;

using Discord.Interactions;
using Discord.WebSocket;
using Modals;

public class ReportCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("report", "Report a user")]
    public async Task Report([Summary("User", "The user you are reporting.")] SocketUser user) =>
        await RespondWithModalAsync(ReportUserModal.ReportModal(user.Id));
}