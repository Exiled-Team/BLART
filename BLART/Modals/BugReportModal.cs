namespace BLART.Modals;

using Commands;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Modules;
using Services;

public static class BugReportModal
{
    private static TextInputBuilder ExiledVersion { get; } =
        new("Exiled Version", "exiled", placeholder: "5.2.0", required: true);

    private static TextInputBuilder Description { get; } =
        new("Bug Description", "description", TextInputStyle.Paragraph, "A description of the issue.", 10, 100, true);

    private static TextInputBuilder Errors { get; } =
        new("Console Errors", "errors", TextInputStyle.Paragraph, "Related error messages in the console.", required: false);

    private static TextInputBuilder DuplicateChannelId { get; } =
        new("Original Channel ID", "channelId", required: true);

    private static ButtonBuilder ConfirmButton { get; } = new("Confirm", "confirm", ButtonStyle.Success);

    private static ButtonBuilder CancelButton { get; } = new("Cancel", "cancel", ButtonStyle.Danger);

    private static MessageComponent ReportButtons { get; } = new ComponentBuilder()
        .WithButton(ConfirmButton)
        .WithButton(CancelButton)
        .Build();

    public static Modal ReportModal { get; } = new ModalBuilder()
        .WithTitle("Bug Report (EXILED bugs only)")
        .WithCustomId("bugreport1")
        .AddTextInput(ExiledVersion)
        .AddTextInput(Description)
        .AddTextInput(Errors)
        .Build();

    private static Modal DuplicateModal(ulong messageId) => new ModalBuilder()
        .WithTitle("Duplicate Bug Linking")
        .WithCustomId($"bugreport2|{messageId}")
        .AddTextInput(DuplicateChannelId)
        .Build();

    private static ButtonBuilder ClaimButton(ulong messageId) => new("Claim", $"claim|{messageId}", ButtonStyle.Primary);
    private static ButtonBuilder SolveButton(ulong messageId) => new("Solved", $"solve|{messageId}", ButtonStyle.Success);
    private static ButtonBuilder InvalidButton(ulong messageId) => new("Invalid", $"invalid|{messageId}", ButtonStyle.Danger);
    private static ButtonBuilder DuplicateButton(ulong messageId) => new("Duplicate", $"duplicate|{messageId}", ButtonStyle.Secondary);

    public static MessageComponent StaffButtons(ulong messageId) => new ComponentBuilder()
        .WithButton(ClaimButton(messageId))
        .WithButton(SolveButton(messageId))
        .WithButton(InvalidButton(messageId))
        .WithButton(DuplicateButton(messageId))
        .Build();

    private static async Task HandleReport(SocketModal modal)
    {
        if (modal.Data.CustomId != ReportModal.CustomId)
            return;
        string exiledVersion = " ";
        string description = " ";
        string errors = "No Information Given";

        foreach (SocketMessageComponentData? component in modal.Data.Components)
        {
            if (string.IsNullOrEmpty(component.Value))
                continue;

            if (component.CustomId == ExiledVersion.CustomId)
                exiledVersion = component.Value;
            else if (component.CustomId == Description.CustomId)
                description = component.Value;
            else if (component.CustomId == Errors.CustomId)
                errors = component.Value;
        }

        EmbedBuilder builder = new();
        builder.WithAuthor(modal.User);
        builder.WithColor(Color.Red);
        builder.WithCurrentTimestamp();
        builder.WithFooter(EmbedBuilderService.Footer);
        builder.WithTitle("Bug Report");
        builder.AddField("Exiled Version", exiledVersion);
        builder.WithDescription(description);
        builder.AddField("Errors", errors);

        BugReporting.BugReports.TryAdd(modal.User, builder);

        await modal.RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Bug Report",
            "Your bug report has been saved.\nPlease note that bug reports should only be sent for **EXILED** issues, and not **PLUGIN** bugs.\nIf you are certain this issue is not being caused by a plugin instead of EXILED, you can use `/bug confirm` to send the bug report.\nTo cancel your report before submitting it, use `/bug cancel`.",
            Color.Red), ephemeral: true, components: ReportButtons);
    }

    private static async Task HandleClaim(SocketMessageComponent component, IUserMessage message)
    {
        IEmbed embed = message.Embeds.First();
        await message.ModifyAsync(x =>
        {
            EmbedBuilder builder = new();
            builder.WithTitle($"Fix in progress by: {component.User.Username} - {embed.Title}");
            builder.WithDescription(embed.Description);
            builder.WithCurrentTimestamp();
            builder.WithFooter(embed.Footer!.Value.Text);
            builder.WithColor(Color.Orange);
            builder.WithAuthor(new EmbedAuthorBuilder()
            {
                Name = embed.Author?.Name ?? $"{component.User.Username}#{component.User.Discriminator}"
            });
            foreach (EmbedField field in embed.Fields)
                builder.Fields.Add(new EmbedFieldBuilder
                {
                    IsInline = field.Inline,
                    Name = field.Name,
                    Value = field.Value
                });
            x.Embed = builder.Build();
        });

        await BugReporting.OpenThreads[message.Id].SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed(
            "Fix in progress",
            $"{component.User.Username} has marked this bug as `Fix in Progress`. Further testing results etc should be directed to them.",
            Color.Gold));
        await component.RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Bug Report Claim", "The bug has been claimed by you.", Color.Green), ephemeral: true);
    }

    private static async Task HandleSolve(SocketMessageComponent component, IUserMessage message)
    {
        IEmbed embed = message.Embeds.First();
        await message?.ModifyAsync(x =>
        {
            EmbedBuilder builder = new();
            builder.WithTitle($"Marked SOLVED by: {component.User.Username}");
            builder.WithDescription(embed.Description);
            builder.WithCurrentTimestamp();
            builder.WithFooter(embed.Footer!.Value.Text);
            builder.WithColor(Color.Gold);
            builder.WithAuthor(new EmbedAuthorBuilder()
            {
                Name = embed.Author?.Name ?? $"{component.User.Username}#{component.User.Discriminator}"
            });
            foreach (EmbedField field in embed.Fields)
                builder.Fields.Add(new EmbedFieldBuilder
                {
                    IsInline = field.Inline,
                    Name = field.Name,
                    Value = field.Value
                });
            x.Embed = builder.Build();
        })!;
        
        await BugReporting.OpenThreads[message.Id].ModifyAsync(x => x.Archived = true);
        await BugReporting.OpenThreads[message.Id].SendMessageAsync(
            embed: await EmbedBuilderService.CreateBasicEmbed("Bug Solved",
                $"This bug has been marked solved by {component.User.Username}.", Color.Gold));
        DatabaseHandler.RemoveEntry(message.Id, DatabaseType.BugReport);
        await component.RespondAsync(
            embed: await EmbedBuilderService.CreateBasicEmbed("Bug Solving", "The bug has been marked as solved.",
                Color.Green), ephemeral: true);
    }

    private static async Task HandleInvalid(SocketMessageComponent component, IUserMessage message)
    {
        await BugReporting.OpenThreads[message.Id].DeleteAsync();
        await message.DeleteAsync();
        await component.RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Bug Report Invalidation", "The given bug report has been deleted.", Color.Red), ephemeral: true);
    }

    private static async Task HandleDuplicate(SocketMessageComponent component, IUserMessage message)
    {
        if (!await CheckPermissions(component, message))
            return;
        
        await component.RespondWithModalAsync(DuplicateModal(message.Id));
    }

    private static async Task HandleDuplicate(SocketModal modal, IUserMessage message, ulong originalId)
    {
        SocketThreadChannel duplicateThread = BugReporting.OpenThreads[message.Id];
        SocketThreadChannel originalThread = BugReporting.OpenThreads[originalId];

        await duplicateThread.JoinAsync();
        await duplicateThread.SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Duplicate Bug Report",
            $"This thread has been marked as a duplicate of <#{originalThread.Id}> and has been locked.", Color.Gold));
        await duplicateThread.ModifyAsync(x =>
        {
            x.Locked = true;
            x.Archived = true;
        });

        await message.DeleteAsync();
        await modal.RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Bug Duplicate Marking",
            $"The selected bug report was marked as a duplicate of <#{originalThread.Id}>.", Color.Green), ephemeral: true);
    }

    private static async Task HandleConfirm(SocketMessageComponent component)
    {
        EmbedBuilder builder = BugReporting.BugReports[component.User];

        IUserMessage message = await BugReporting.BugReportChannel.SendMessageAsync(embed: builder.Build());
        await message.ModifyAsync(x =>
        {
            x.Components = StaffButtons(message.Id);
        });

        BugReporting.BugReports.Remove(component.User);
        BugReporting.OpenThreads.Add(message.Id, await BugReporting.BugReportChannel.CreateThreadAsync(builder.Description.Length > 23 ? builder.Description[..23] : builder.Description, message: message, autoArchiveDuration: ThreadArchiveDuration.ThreeDays, invitable: true));
        DatabaseHandler.AddEntry(message.Id, BugReporting.OpenThreads[message.Id].Id.ToString(), DatabaseType.BugReport);
        await component.RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Bug Report Confirmation", "Your bug has been submitted. Don't forget to add any screenshots/videos/logfiles to the thread to provide additional information.", Color.Green), ephemeral: true);
    }

    private static async Task HandleCancel(SocketMessageComponent component)
    {
        BugReporting.BugReports.Remove(component.User);
        await component.RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Bug Report Cancelling", "Your bug report has been cancelled.", Color.Green), ephemeral: true);
    }

    private static async Task<bool> CheckPermissions(SocketInteraction interaction, IUserMessage message)
    {
        if ($"{interaction.User.Username}#{interaction.User.Discriminator}" != message.Embeds.First().Author?.Name && !CommandHandler.CanRunStaffCmd(interaction.User))
        {
            await interaction.RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied,
                "Only the bug submitter, Discord Staff and EXILED Developers can modify existing bug reports."), ephemeral: true);
            return false;
        }

        return true;
    }

    public static async Task HandleModal(SocketModal modal)
    {
        if (modal.Data.CustomId == ReportModal.CustomId)
            await HandleReport(modal);
        else if (modal.Data.CustomId.Contains('|') && modal.Data.CustomId.Contains(DuplicateModal(0).CustomId.Replace("|0", string.Empty)))
        {
            IUserMessage message = (IUserMessage) await modal.Channel.GetMessageAsync(ulong.Parse(modal.Data.CustomId.AsSpan(modal.Data.CustomId.IndexOf('|'), 18)));
            if (message is null)
            {
                await modal.RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Bug Report Duplicate Marking", "Unable to locate the message of the modal. Blame discord devs for being dipshits.", Color.Red), ephemeral: true);
                return;
            }

            if (!await CheckPermissions(modal, message))
                return;

            if (!ulong.TryParse(modal.Data.Components.FirstOrDefault()?.Value, out ulong originalId) || !BugReporting.OpenThreads.ContainsKey(originalId))
            {
                await modal.RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Bug report Duplicate Marking", "You provided an invalid original message ID.", Color.Red), ephemeral: true);
                return;
            }

            await HandleDuplicate(modal, message, originalId);
        }
    }

    public static async Task HandleButton(SocketMessageComponent component)
    {
        if (component.Data.CustomId == ConfirmButton.CustomId)
        {
            await HandleConfirm(component);
            return;
        }

        if (component.Data.CustomId == CancelButton.CustomId)
        {
            await HandleCancel(component);
            return;
        }

        ulong messageId = 0;

        if (component.Data.CustomId.Contains('|'))
        {
            try
            {
                if (!ulong.TryParse(component.Data.CustomId.AsSpan(component.Data.CustomId.IndexOf('|') + 1, 18), out messageId))
                {
                    Log.Error(nameof(HandleModal),
                        $"Unable to parse Message ID from {component.Data.CustomId}\n{component.Data.CustomId.Substring(component.Data.CustomId.IndexOf('|', 18))}");
                    await component.RespondAsync(
                        embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.Unspecified),
                        ephemeral: true);
                    return;
                }
            }
            catch (Exception)
            {
                await component.RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.Unspecified), ephemeral: true);
            }
        }

        Log.Debug(nameof(HandleButton), $"Got message ID: {messageId}");
        IUserMessage message = (IUserMessage)await component.Channel.GetMessageAsync(messageId);
        string customId = component.Data.CustomId.Replace($"|{messageId}", string.Empty);
        string claimId = ClaimButton(0).CustomId.Replace("|0", string.Empty);
        string solveId = SolveButton(0).CustomId.Replace("|0", string.Empty);
        string invalidId = InvalidButton(0).CustomId.Replace("|0", string.Empty);
        string duplicateId = DuplicateButton(0).CustomId.Replace("|0", string.Empty);

        if (!await CheckPermissions(component, message))
            return;

        if (customId == claimId)
            await HandleClaim(component, message);
        else if (customId == solveId)
            await HandleSolve(component, message);
        else if (customId == invalidId)
            await HandleInvalid(component, message);
        else if (customId == duplicateId)
            await HandleDuplicate(component, message);
    }
}