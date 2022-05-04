namespace BLART.Commands;

using Discord.Interactions;
using Modals;

[Group("plugin", "Commands to control plugin channels.")]
public class PluginCommands : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("submit", "Create a plugin channel.")]
    public async Task SubmitPlugin() => await RespondWithModalAsync(PluginSubmissionModal.EmbedModal);
}