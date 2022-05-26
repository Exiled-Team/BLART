namespace BLART.Modals;

using Discord;
using Discord.Net;
using Discord.WebSocket;
using Services;

public class ReportUserModal
{
    private static TextInputBuilder Messages { get; } = new("Messages", "messages", TextInputStyle.Short,
        "The message links to be reported.", required: true);

    private static TextInputBuilder Reason { get; } = new("Reason", "reason", TextInputStyle.Paragraph,
        "A description of why you are reporting these messages", required: true);

    public static Modal ReportModal(ulong userId) => new ModalBuilder()
        .WithTitle("User Report")
        .WithCustomId($"userreport|{userId}")
        .AddTextInput(Messages)
        .AddTextInput(Reason)
        .Build();

    public static async Task HandleModal(SocketModal modal)
    {
        try
        {
            Log.Info(nameof(HandleModal), modal.Data.CustomId);
            if (!modal.Data.CustomId.Contains('|') || !modal.Data.CustomId.Contains(ReportModal(0).CustomId.Replace("|0", string.Empty)))
                return;
            Log.Info(nameof(HandleModal), "Handling user report");
            if (!ulong.TryParse(modal.Data.CustomId.AsSpan(modal.Data.CustomId.IndexOf('|') + 1, 18), out ulong userId))
            {
                await modal.RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.UnableToParseId,
                    "Failed to parse message ID from embed."), ephemeral: true);
                return;
            }

            SocketGuildUser user = Bot.Instance.Guild.GetUser(userId);
            if (user is null)
            {
                await modal.RespondAsync(
                    embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.UnableToParseId,
                        "Failed to parse user ID from embed."), ephemeral: true);
                return;
            }

            string messageLinks = " ";
            string reason = " ";

            foreach (SocketMessageComponentData? component in modal.Data.Components)
            {
                if (string.IsNullOrEmpty(component.Value))
                    continue;

                if (component.CustomId == Messages.CustomId)
                    messageLinks = component.Value;
                else if (component.CustomId == Reason.CustomId)
                    reason = component.Value;
            }

            EmbedBuilder builder = new();
            builder.WithTitle("New user report");
            builder.WithFooter(EmbedBuilderService.Footer);
            builder.WithColor(Color.Red);
            builder.AddField("Reported User", user.Mention);
            builder.AddField("Message(s)", messageLinks);
            builder.WithDescription(reason);

            Log.Info(nameof(HandleModal), "Sending report message");
            await Bot.Instance.Guild.GetTextChannel(Program.Config.StaffChannelId)
                .SendMessageAsync($"{Bot.Instance.Guild.GetRole(Program.Config.DiscStaffId).Mention}",
                    embed: builder.Build());

            Log.Info(nameof(HandleModal), "Sending response.");
            await modal.RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Report Created",
                "Your report has been accepted and anonymously sent to Discord Staff.", Color.Gold), ephemeral: true);
        }
        catch (Exception e)
        {
            Log.Error(nameof(HandleModal), e);
            await modal.RespondAsync(
                embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.Unspecified, e.Message), ephemeral: true);
        }
    }
}