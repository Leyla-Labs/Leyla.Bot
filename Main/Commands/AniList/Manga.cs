using Anilist4Net;
using Anilist4Net.Enums;
using Common.Classes;
using Common.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Main.Helper;

namespace Main.Commands.AniList;

public sealed class Manga : SlashCommand
{
    private readonly string _title;

    public Manga(InteractionContext ctx, string title) : base(ctx)
    {
        _title = title;
    }

    public override async Task RunAsync()
    {
        await Ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var manga = int.TryParse(_title, out var result)
            ? await new Client().GetMediaById(result)
            : await new Client().GetMediaBySearch(_title, MediaTypes.MANGA);

        if (manga == null ||
            !new[] {MediaFormats.MANGA, MediaFormats.NOVEL, MediaFormats.ONE_SHOT}.Contains(manga.Format))
        {
            await Ctx.EditResponseAsync(
                new DiscordWebhookBuilder().AddErrorEmbed($"\"{_title}\" not found"));
            return;
        }

        var embed = new DiscordEmbedBuilder();
        AniListHelper.AddCommonMediaFieldsTop(embed, manga);
        AddFields(embed, manga);
        AniListHelper.AddCommonMediaFieldsBottom(embed, manga);
        AniListHelper.AddCommonMediaEmbedProperties(embed, manga);
        var btn = AniListHelper.GetSiteButton(manga);
        await Ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed.Build()).AddComponents(btn));
    }

    private static void AddFields(DiscordEmbedBuilder embed, Media media)
    {
        if (media.Volumes > 0)
        {
            embed.AddField("Volumes", media.Volumes.ToString(), true);
        }

        if (media.Chapters > 0)
        {
            embed.AddField("Chapters", media.Chapters.ToString(), true);
        }
    }
}