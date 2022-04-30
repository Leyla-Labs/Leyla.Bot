using DSharpPlus;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Main.Commands.AniList;

namespace Main.Modules;

[SlashCommandGroup("AniList", "Get various data from AniList: Anime TODO")]
public class AniList : ApplicationCommandModule
{
    [SlashCommand("anime", "Shows information for the given anime.")]
    [SlashRequireBotPermissions(Permissions.SendMessages | Permissions.EmbedLinks)]
    public async Task SlashAnime(InteractionContext ctx,
        [Option("Title", "Title of the anime to search for")]
        string title)
    {
        await Anime.RunSlash(ctx, title);
    }

    [SlashCommand("manga", "Shows information for the given manga.")]
    [SlashRequireBotPermissions(Permissions.SendMessages | Permissions.EmbedLinks)]
    public async Task SlashManga(InteractionContext ctx,
        [Option("Title", "Title of the manga to search for")]
        string title)
    {
        await Manga.RunSlash(ctx, title);
    }

    [SlashCommand("character", "Shows information for the given character.")]
    [SlashRequireBotPermissions(Permissions.SendMessages | Permissions.EmbedLinks)]
    public async Task SlashCharacter(InteractionContext ctx,
        [Option("Name", "Name of the character to search for")]
        string title)
    {
        await Character.RunSlash(ctx, title);
    }
}