using Anilist4Net;
using Common.Classes;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Main.Extensions;

namespace Main.Commands.AniList;

public sealed class Character : SlashCommand
{
    private readonly string _name;

    public Character(InteractionContext ctx, string name) : base(ctx)
    {
        _name = name;
    }

    public override async Task RunAsync()
    {
        await Ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        var character = await new Client().GetCharacterBySearch(_name);

        var embed = new DiscordEmbedBuilder();
        embed.WithTitle(character.FullName);
        embed.WithDescription(character.NativeName);

        var altNames = string.Join(", ", character.AlternativeNames);
        if (altNames.Length > 0)
        {
            embed.AddField("Alternative Names", altNames);
        }

        embed.AddField("Description",
            character.DescriptionMd.StripHtml().ToDiscordMarkup().TruncateAndCloseSpoiler(210));
        embed.WithThumbnail(character.ImageLarge);
        embed.WithFooter("AniList", "https://i.imgur.com/zqa6OEk.png");
        embed.WithColor(2010108);
        var btn = new DiscordLinkButtonComponent(character.SiteUrl, "View all details on AniList");
        await Ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed.Build()).AddComponents(btn));
    }
}