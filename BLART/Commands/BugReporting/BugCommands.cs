namespace BLART.Commands.BugReporting;

using Discord.Commands;
using Discord.Interactions;
using Modals;
using Modules;
using Group = Discord.Interactions.GroupAttribute;
using Summary = Discord.Interactions.SummaryAttribute;

[Group("bug", "Commands for managing bug reports.")]
public partial class BugCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("report", "Report a bug in EXILED.")]
    public async Task ReportBug() => await RespondWithModalAsync(BugReportModal.ReportModal);
}