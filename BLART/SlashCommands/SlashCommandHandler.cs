using System.Reflection;
using BLART.Services;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;

namespace BLART.SlashCommands
{
    public class SlashCommandHandler
    {
        private readonly InteractionService service;
        private readonly DiscordSocketClient client;

        public SlashCommandHandler(InteractionService interaction, DiscordSocketClient client)
        {
            this.service = interaction;
            this.client = client;
        }

        public async Task InstallCommandsAsync()
        {
            await service.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            client.InteractionCreated += HandleInteraction;
            service.SlashCommandExecuted += SlashServiceExecuted;
            service.ContextCommandExecuted += ContextServiceExecuted;
            service.ComponentCommandExecuted += ComponentServiceExecuted;
        }
        
        private async Task ComponentServiceExecuted (ComponentCommandInfo arg1, Discord.IInteractionContext arg2, Discord.Interactions.IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                if (arg3.Error != InteractionCommandError.UnknownCommand)
                {
                    return;
                }
                await arg2.Interaction.RespondAsync("", new Embed[1] { await ErrorHandlingService.GetErrorEmbed(ErrorCodes.Unspecified, arg3.ErrorReason) } );
            }
        }

        private async Task ContextServiceExecuted (ContextCommandInfo arg1, Discord.IInteractionContext arg2, Discord.Interactions.IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                if (arg3.Error != InteractionCommandError.UnknownCommand)
                {
                    return;
                }
                await arg2.Interaction.RespondAsync("", new Embed[1] { await ErrorHandlingService.GetErrorEmbed(ErrorCodes.Unspecified, arg3.ErrorReason) } );
            }
        }

        private async Task SlashServiceExecuted(SlashCommandInfo arg1, IInteractionContext arg2, Discord.Interactions.IResult arg3)
        {
            if (!arg3.IsSuccess)
            {
                if (arg3.Error != InteractionCommandError.UnknownCommand)
                {
                    return;
                }
                await arg2.Interaction.RespondAsync("", new Embed[1] { await ErrorHandlingService.GetErrorEmbed(ErrorCodes.Unspecified, arg3.ErrorReason) } );
            }
        }

        private async Task HandleInteraction (SocketInteraction arg)
        {
            try
            {
                var ctx = new SocketInteractionContext(client, arg);
                await service.ExecuteCommandAsync(ctx, null);
            }
            catch (Exception ex)
            {
                Log.Error(nameof(SlashCommandHandler), ex);
                if(arg.Type == InteractionType.ApplicationCommand)
                    await arg.RespondAsync("", new Embed[1] { await ErrorHandlingService.GetErrorEmbed(ErrorCodes.Unspecified, ex.Message) } );
            }
        }
    }
}