using Discord;
using Discord.Interactions;

namespace BLART.SlashCommands.Sample
{
    [Discord.Interactions.Group("example", "Command group for example commands")]
    public class ExampleCommand : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("example1", "Example Command NR1")]
        public async Task Example1()
        {
            await DeferAsync(false); //you need to respond OR defer in 3 seconds, use defer if this command takes long
            await Task.Delay(4000); //queue heavy thinking

            await FollowupAsync("Hi im done now."); //If you already acknowledged the interact, you need to Followup.
        }
        
        [SlashCommand("example2", "Example Command NR2")]
        public async Task Example2()
        {
            await RespondAsync("Hi, i got it. gonna think now"); //Respond at first

            await Task.Delay(3000); //do stuff
            
            await FollowupAsync("Hi im done now."); //If you already acknowledged the interact, you need to Followup.
        }
        
        [SlashCommand("example3", "Example Command NR3")]
        public async Task Example3(string text)
        {
            await RespondAsync($"{Context.User.Mention}: {text}");
        }
        
        [SlashCommand("example4", "Example Command NR4")]
        public async Task Example4(ITextChannel channel, string text)
        {
            await channel.SendMessageAsync($"{Context.User.Mention}: {text}");
            await RespondAsync($"Done!", ephemeral: true); //reply as invisible message that only the invoker can see
        }
        
        [SlashCommand("example5", "Example Command NR5")]
        public async Task Example5(IGuildUser user, string text)
        {
            await RespondAsync($"{Context.User.Mention} has ponged you {user.Mention}: {text}");
        }
        
        [SlashCommand("example6", "Example Command NR6")]
        public async Task Example6(ITextChannel channel, IGuildUser user, string text)
        {
            await channel.SendMessageAsync($"{Context.User.Mention} has ponged you {user.Mention}: {text}");
            await RespondAsync($"Done!", ephemeral: true); //reply as invisible message that only the invoker can see
        }
        
        [SlashCommand("example7", "Example Command NR7")]
        public async Task Example7(IGuildUser user, IAttachment attachment, string text)
        {
            await DeferAsync();
            HttpClient client = new HttpClient();
            var stream = await client.GetStreamAsync(attachment.Url);
            await FollowupWithFileAsync(new FileAttachment(stream, attachment.Filename), $"{Context.User.Mention} has ponged you {user.Mention}: {text}");
        }
    }
}