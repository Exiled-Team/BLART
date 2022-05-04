namespace BLART.Modals;

using System.Security.Cryptography;
using Commands;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Services;

public static class PluginSubmissionModal
{
    private static TextInputBuilder Title { get; } =
        new("Title", "title", TextInputStyle.Short, "A title for the plugin.", required: true);

    private static TextInputBuilder Repository { get; } = new("Repository", "repo", TextInputStyle.Short,
        "A link to the plugin's github/gitlab repo", required: true);

    private static TextInputBuilder Description { get; } = new("Description", "description", TextInputStyle.Paragraph,
        "A description of your plugin.", required: true);

    private static TextInputBuilder Category { get; } = new("Select Category", "category", TextInputStyle.Short,
        "The category ID for this plugin.", required: true);

    private static ButtonBuilder EditButton { get; } = new("Edit", "plugin-edit", ButtonStyle.Primary);

    private static ButtonBuilder DeleteButton { get; } = new("Delete", "plugin-cancel", ButtonStyle.Danger);

    private static ButtonBuilder AcceptButton { get; } = new("Accept", "plugin-accept", ButtonStyle.Success);
    
    public static Modal EmbedModal { get; } = new ModalBuilder()
        .WithTitle("Submit Plugin")
        .WithCustomId("plugin-embed")
        .AddTextInput(Title)
        .AddTextInput(Repository)
        .AddTextInput(Description)
        .Build();

    private static Modal EditEmbedModal(ulong messageId) => new ModalBuilder()
        .WithTitle("Edit plugin submission")
        .WithCustomId($"plugin-edit|{messageId}")
        .AddTextInput(Title)
        .AddTextInput(Repository)
        .AddTextInput(Description)
        .Build();

    private static Modal SelectCategory(ulong messageId) => new ModalBuilder()
        .WithTitle("Select Plugin Category")
        .WithCustomId($"plugin-select|{messageId}")
        .AddTextInput(Category)
        .Build();

    private static async Task<Embed?> ConstructEmbed(SocketModal modal)
    {
        string title = string.Empty;
        string repository = string.Empty;
        string description = string.Empty;

        foreach (SocketMessageComponentData? input in modal.Data.Components)
        {
            if (input.CustomId == Title.CustomId)
                title = input.Value;
            else if (input.CustomId == Repository.CustomId)
                repository = input.Value;
            else if (input.CustomId == Description.CustomId)
                description = input.Value;
        }

        EmbedBuilder builder = new();
        builder.WithTitle(title);
        builder.WithAuthor(modal.User);
        builder.WithFooter(EmbedBuilderService.Footer);
        builder.WithCurrentTimestamp();
        builder.WithColor(Color.Teal);
        builder.WithDescription(description);
        builder.AddField("Plugin Repository", repository);

        return builder.Build();
    }

    private static async Task<bool> CanUse(SocketMessageComponent component, bool strict = false)
    {
        if (!CommandHandler.CanRunStaffCmd(component.User) && (!strict && component.Message.Embeds.FirstOrDefault()?.Author?.Name != $"{component.User.Username}#{component.User.Discriminator}"))
        {
            await component.RespondAsync($"{component.User.Mention}", embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            
            return false;
        }

        return true;
    }

    private static async Task HandleCreateSubmission(SocketModal modal)
    {
        Embed? embed = await ConstructEmbed(modal);
        if (embed is null)
            return;

        SocketTextChannel channel = Bot.Instance.Guild.GetTextChannel(695423213185794059);
        IUserMessage message = await channel.SendMessageAsync(embed: embed);
        
        await message.ModifyAsync(x => x.Components = new ComponentBuilder().WithButton(EditButton).WithButton(DeleteButton).WithButton(AcceptButton).Build());
        await modal.RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Plugin Submitted", "The plugin has been submitted.", Color.Green), ephemeral: true);
    }

    private static async Task HandleEditPlugin(SocketModal modal)
    {
        Embed? embed = await ConstructEmbed(modal);
        if (embed is null)
            return;

        if (!ulong.TryParse(modal.Data.CustomId.AsSpan(modal.Data.CustomId.IndexOf('|') + 1, 18), out ulong messageId))
        {
            await modal.RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.UnableToParseId, "Failed to parse message ID from embed."), ephemeral: true);
            
            return;
        }

        IUserMessage message = (IUserMessage) await modal.Channel.GetMessageAsync(messageId);
        
        await message.ModifyAsync(x => x.Embed = embed);
        await modal.RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Submission Edited", "The plugin submission has been edited", Color.Green), ephemeral: true);
    }

    private static async Task HandleSelectCategory(SocketModal modal)
    {
        if (!ulong.TryParse(modal.Data.CustomId.AsSpan(modal.Data.CustomId.IndexOf('|') + 1, 18), out ulong messageId))
        {
            await modal.RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.UnableToParseId, "Unable to parse message ID from embed."), ephemeral: true);
            
            return;
        }

        if (!ulong.TryParse(modal.Data.Components.FirstOrDefault()?.Value, out ulong categoryId))
        {
            await modal.RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.UnableToParseId, "Unable to parse category ID given."), ephemeral: true);
            
            return;
        }

        await modal.DeferAsync();
        
        IUserMessage? message = (IUserMessage)await modal.Channel.GetMessageAsync(messageId);
        Embed? embed = (Embed)message.Embeds.FirstOrDefault()!;
        SocketCategoryChannel? category = Bot.Instance.Guild.GetCategoryChannel(categoryId);
        if (category is null)
        {
            await modal.RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.InvalidChannelId,
                "The category ID is invalid."), ephemeral: true);
            return;
        }

        IGuildUser? user = (await Bot.Instance.Guild.GetUsersAsync().FlattenAsync()).FirstOrDefault(u => $"{u.Username}#{u.Discriminator}" == embed.Author?.Name);
        
        
        RestTextChannel channel = await Bot.Instance.Guild.CreateTextChannelAsync(embed.Title);
        await channel.AddPermissionOverwriteAsync(Bot.Instance.Guild.EveryoneRole, new OverwritePermissions(sendMessages: PermValue.Deny));
        
        if (user is not null)
            await channel.AddPermissionOverwriteAsync(user, new OverwritePermissions(manageChannel: PermValue.Allow, sendMessages: PermValue.Allow, manageWebhooks: PermValue.Allow));
        
        IUserMessage newMessage = await channel.SendMessageAsync(embed: embed);
        RestRole? role = await Bot.Instance.Guild.CreateRoleAsync(embed.Title);
        
        await channel.ModifyAsync(x =>
        {
            x.CategoryId = category.Id;
        });
        await newMessage.ModifyAsync(x => x.Components = new ComponentBuilder().WithButton(EditButton).WithButton(DeleteButton).Build());
        await message.DeleteAsync();
        DatabaseHandler.AddEntry(role.Id, string.Empty, DatabaseType.SelfRole);
        
        await modal.FollowupAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Plugin Accepted", "The plugin has been accepted.", Color.Green), ephemeral: true);
    }
    
    public static async Task HandleButton(SocketMessageComponent component)
    {
        if (component.Data.CustomId == EditButton.CustomId)
        {
            if (await CanUse(component))
                await component.RespondWithModalAsync(EditEmbedModal(component.Message.Id));
        }
        else if (component.Data.CustomId == DeleteButton.CustomId)
        {
            if (await CanUse(component))
            {
                await component.Message.DeleteAsync();
                await component.RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Submission Cancelled", "The plugin submission has been removed.", Color.Green), ephemeral: true);
            }
        }
        else if (component.Data.CustomId == AcceptButton.CustomId)
        {
            if (((IGuildUser) component.User).RoleIds.All(r => r != 656673336402640902))
                await component.RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            else
                await component.RespondWithModalAsync(SelectCategory(component.Message.Id));
        }
    }

    public static async Task HandleModal(SocketModal modal)
    {
        string customId = modal.Data.CustomId;
        if (customId.Contains('|'))
        {
            string toRemove = modal.Data.CustomId.Substring(modal.Data.CustomId.IndexOf('|'), 19);
            customId = modal.Data.CustomId.Replace(toRemove, string.Empty);
            Log.Info(nameof(HandleModal), customId);
        }

        if (modal.Data.CustomId == EmbedModal.CustomId)
            await HandleCreateSubmission(modal);
        else if (customId == EditEmbedModal(0).CustomId.Replace("|0", string.Empty))
            await HandleEditPlugin(modal);
        else if (customId == SelectCategory(0).CustomId.Replace("|0", string.Empty))
            await HandleSelectCategory(modal);
    }
}