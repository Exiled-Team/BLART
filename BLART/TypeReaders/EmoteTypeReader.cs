namespace BLART.TypeReaders;

using Discord;
using Discord.Commands;

public class EmoteTypeReader : TypeReader
{
    public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
    {
        if (Emote.TryParse(input, out Emote emote))
            return Task.FromResult(TypeReaderResult.FromSuccess(emote));
        if (Emoji.TryParse(input, out Emoji emoji))
            return Task.FromResult(TypeReaderResult.FromSuccess(emoji));

        return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed,
            "Input could not be parsed as an IEmote."));
    }
}