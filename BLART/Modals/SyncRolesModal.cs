namespace BLART.Modals;

using Discord;
using Discord.WebSocket;
using Services;

public static class SyncRolesModal
{
    public static List<ulong> Roles { get; } = new();

    private static ButtonBuilder YesButton(ulong role) => new("Yes", $"yes|{role}", ButtonStyle.Success);
    private static ButtonBuilder NoButton(ulong role) => new("No", $"no|{role}", ButtonStyle.Danger);

    public static MessageComponent SyncComponent(ulong role) => new ComponentBuilder().WithButton(YesButton(role)).WithButton(NoButton(role)).Build();

    private static async Task HandleNo(SocketMessageComponent component)
    {
        if (ulong.TryParse(component.Data.CustomId.AsSpan(component.Data.CustomId.IndexOf('|') + 1, 18), out ulong roleId))
        {
            Roles.Remove(roleId);
            if (Roles.Count == 0)
            {
                await component.RespondAsync(
                    embed: await EmbedBuilderService.CreateBasicEmbed("Unassignable",
                        $"{roleId} marked as not assignable. There are no more roles to check.", Color.Green),
                    ephemeral: true);
                return;
            }
            ulong newRoleId = Roles.FirstOrDefault();
            await component.RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Unassignable", $"{roleId} marked as not assignable.\nIs <@&{newRoleId}> self-assignable?", Color.Green), ephemeral: true, components: SyncComponent(newRoleId));
        }
    }

    private static async Task HandleYes(SocketMessageComponent component)
    {
        if (ulong.TryParse(component.Data.CustomId.AsSpan(component.Data.CustomId.IndexOf('|') + 1, 18), out ulong roleId))
        {
            Roles.Remove(roleId);
            DatabaseHandler.AddEntry(roleId, string.Empty, DatabaseType.SelfRole);
            if (Roles.Count == 0)
            {
                await component.RespondAsync(
                    embed: await EmbedBuilderService.CreateBasicEmbed("Unassignable",
                        $"{roleId} marked as not assignable. There are no more roles to check.", Color.Green),
                    ephemeral: true);
                return;
            }
            ulong newRoleId = Roles.FirstOrDefault();
            await component.RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Assignable", $"{roleId} marked as self-assignable.\nIs <@&{newRoleId}> self-assignable?", Color.Green), ephemeral: true, components: SyncComponent(newRoleId));
        }
    }

    public static async Task HandleButton(SocketMessageComponent component)
    {
        if (component.Data.CustomId.Contains("yes"))
            await HandleYes(component);
        else if (component.Data.CustomId.Contains("no"))
            await HandleNo(component);
    }
}