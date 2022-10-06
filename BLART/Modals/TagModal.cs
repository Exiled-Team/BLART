namespace BLART.Modals;

using BLART.Objects;
using BLART.Services;

using Discord;
using Discord.WebSocket;

public class TagModal
{
    private static TextInputBuilder Title { get; } = new("Name", "name", TextInputStyle.Short, "A name for the tag.", required: true);

    private static TextInputBuilder Text { get; } = new("Text", "text", TextInputStyle.Paragraph, "The text for the tag.", required: true);

    public static Modal CreateTagModal() => new ModalBuilder().WithTitle("Create Tag").AddTextInput(Title).AddTextInput(Text).WithCustomId("tag").Build();

    public static Modal EditTagModal(string name) => new ModalBuilder().WithTitle("Update Tag").AddTextInput(Text).WithCustomId($"edittag|{name}").Build();

    public static async Task HandleModal(SocketModal modal)
    {
        string customId = modal.Data.CustomId;
        if (customId.Contains('|'))
        {
            string toRemove = modal.Data.CustomId.Substring(modal.Data.CustomId.IndexOf('|'));
            customId = modal.Data.CustomId.Replace(toRemove, string.Empty);
        }

        if (customId == CreateTagModal().CustomId)
        {
            Tag? tag = null;
            string name = string.Empty;
            string text = string.Empty;

            foreach (SocketMessageComponentData? input in modal.Data.Components)
            {
                if (input.CustomId == Title.CustomId)
                {
                    name = input.Value;
                    tag = DatabaseHandler.GetTag(input.Value);
                }
                else if (input.CustomId == Text.CustomId)
                    text = input.Value;
            }

            if (tag is not null)
            {
                await modal.RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.AlreadyExists, "A tag with that name already exists."), ephemeral: true);
                return;
            }

            DatabaseHandler.AddEntry(0, $"{name}|{text}", DatabaseType.Tags);

            await modal.RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Tag Created", "The tag was created successfully!", Color.Green), ephemeral: true);
        }
        else if (customId == EditTagModal("0").CustomId.Replace("|0", string.Empty))
        {
            string name = modal.Data.CustomId.AsSpan(modal.Data.CustomId.IndexOf('|') + 1).ToString();
            Tag? tag = DatabaseHandler.GetTag(name);
            string text = string.Empty;
            foreach (SocketMessageComponentData? input in modal.Data.Components)
            {
                if (input.CustomId == Text.CustomId)
                    text = input.Value;
            }

            if (tag is null)
            {
                await modal.RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.NoRecordFound), ephemeral: true);
                return;
            }

            DatabaseHandler.RemoveEntry(tag.Id, DatabaseType.Tags);
            DatabaseHandler.AddEntry(0, $"{name}|{text}", DatabaseType.Tags);

            await modal.RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Tag Updated", "The specified tag has been updated!", Color.Orange), ephemeral: true);
        }
    }
}