namespace BLART.Modals;

using Commands;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Modules;
using Services;

public static class EmbedModal
{
    private static TextInputBuilder Title { get; } =
        new("Title", "title", TextInputStyle.Short, "A title for the embed.", required: true);

    private static TextInputBuilder Color { get; } = new("Color", "color", TextInputStyle.Short,
        "The HEX color code to use for the embed.", required: true);

    private static TextInputBuilder Body1 { get; } = new("Contents", "content1", TextInputStyle.Paragraph,
        "The contents of the embed.", required: true);

    private static TextInputBuilder Body2 { get; } = new("Contents", "content2", TextInputStyle.Paragraph,
        "Additional content (if needed)", required: false);

    private static TextInputBuilder Body3 { get; } = new("Contents", "content3", TextInputStyle.Paragraph,
        "Additional content (if needed)", required: false);

    public static ButtonBuilder EditButton { get; } = new("Edit", "edit", ButtonStyle.Primary);

    public static Modal Embed(ulong channelId) =>
        new ModalBuilder()
            .WithTitle("Create Embed")
            .WithCustomId($"embed|{channelId}")
            .AddTextInput(Title)
            .AddTextInput(Color)
            .AddTextInput(Body1)
            .AddTextInput(Body2)
            .AddTextInput(Body3)
            .Build();

    public static Modal EditEmbedModal(ulong messageId) =>
        new ModalBuilder()
            .WithTitle("Edit Embed")
            .WithCustomId($"edit|{messageId}")
            .AddTextInput(Title)
            .AddTextInput(Color)
            .AddTextInput(Body1)
            .AddTextInput(Body2)
            .AddTextInput(Body3)
            .Build();

    private static SocketTextChannel? GetChannel(SocketModal modal)
    {
        if (!modal.Data.CustomId.Contains('|'))
            return null;

        if (!ulong.TryParse(modal.Data.CustomId.AsSpan(modal.Data.CustomId.IndexOf('|') + 1, 18), out ulong channelId))
        {
            Log.Error(nameof(GetChannel), $"Unable to parse channel ID from {modal.Data.CustomId} -- {modal.Data.CustomId.Substring(modal.Data.CustomId.IndexOf('|') + 1, 18)}");
            return null;
        }

        return Bot.Instance.Guild.GetTextChannel(channelId);
    }

    private static async Task<Embed?> ConstructEmbed(SocketModal modal)
    {
        if (modal.Data.CustomId.Contains(Embed(0).CustomId.Replace("|0", string.Empty)) ||
            modal.Data.CustomId.Contains(EditEmbedModal(0).CustomId.Replace("|0", string.Empty)))
        {
            string title = string.Empty;
            string colorRaw = string.Empty;
            List<string> contents = new();

            foreach (SocketMessageComponentData? input in modal.Data.Components)
            {
                if (input.CustomId == Title.CustomId)
                    title = input.Value;
                else if (input.CustomId == Color.CustomId)
                    colorRaw = input.Value;
                else if (input.CustomId == Body1.CustomId)
                    contents.Add(input.Value);
                else if (input.CustomId == Body2.CustomId && !string.IsNullOrEmpty(input.Value))
                    contents.Add(input.Value);
                else if (input.CustomId == Body3.CustomId && !string.IsNullOrEmpty(input.Value))
                    contents.Add(input.Value);
            }

            Color color;

            if (colorRaw.Contains('#'))
                colorRaw = colorRaw.Replace("#", string.Empty);

            try
            {
                color = new((uint) ColorParsing.ToColorValue(colorRaw));
            }
            catch (Exception e)
            {
                Log.Error(nameof(HandleModal), $"Color failed to parse: {colorRaw}\n{e}");
                await modal.RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.FailedToParseColor, colorRaw), ephemeral: true);
                return null;
            }

            EmbedBuilder builder = new();
            builder.WithTitle(title);
            builder.WithAuthor(modal.User);
            builder.WithFooter(EmbedBuilderService.Footer);
            builder.WithCurrentTimestamp();
            builder.WithColor(color);
            builder.WithDescription(string.Concat(contents));

            return builder.Build();
        }

        return null;
    }

    public static async Task HandleButton(SocketMessageComponent component)
    {
        if (component.Data.CustomId == EditButton.CustomId)
        {
            if (!CommandHandler.CanRunStaffCmd(component.User, false))
            {
                await component.RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
                return;
            }

            await component.RespondWithModalAsync(EditEmbedModal(component.Message.Id));
        }
    }

    public static async Task HandleModal(SocketModal modal)
    {
        Embed? embed = await ConstructEmbed(modal);
        if (embed is null)
            return;
        
        if (modal.Data.CustomId.Contains(Embed(0).CustomId.Replace("|0", string.Empty)))
        {
            SocketTextChannel? channel = GetChannel(modal);

            if (channel is null)
            {
                await modal.RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.Unspecified,
                    "Unable to parse channel from modal."), ephemeral: true);
                return;
            }
            
            IUserMessage message = await channel.SendMessageAsync(embed: embed);
            await message.ModifyAsync(x =>
            {
                x.Components = new ComponentBuilder().WithButton(EditButton).Build();
            });

            await modal.RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Embed Created", "The embed has been created.", Discord.Color.Green), ephemeral: true);
        }
        else if (modal.Data.CustomId.Contains(EditEmbedModal(0).CustomId.Replace("|0", string.Empty)))
        {
            if (!ulong.TryParse(modal.Data.CustomId.AsSpan(modal.Data.CustomId.IndexOf('|') + 1, 18), out ulong messageId))
            {
                await modal.RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.UnableToParseId,
                    "Failed to parse message ID from embed."), ephemeral: true);
                return;
            }

            IUserMessage message = (IUserMessage)await modal.Channel.GetMessageAsync(messageId);
            await message.ModifyAsync(x => x.Embed = embed);

            await modal.RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Embed Edited", "The embed has been edited.", Discord.Color.Green), ephemeral: true);
        }
    }
}