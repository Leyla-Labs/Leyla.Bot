using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Main.Commands.Quotes;
using Random = Main.Commands.Quotes.Random;

namespace Main.Modules;

[SlashCommandGroup("Quote", "Description TODO")]
[SlashRequireGuild]
internal sealed class Quotes : ApplicationCommandModule
{
    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Add Quote")]
    public async Task MenuAdd(ContextMenuContext ctx)
    {
        await new Add(ctx).RunAsync();
    }

    [SlashCommand("show", "Shows a specific quote.")]
    [SlashRequireBotPermissions(Permissions.SendMessages | Permissions.EmbedLinks)]
    public async Task SlashShow(InteractionContext ctx,
        [Option("user", "User to show quote of")]
        DiscordUser user,
        [Option("n", "Number of the quote to show")]
        long n)
    {
        // TODO check if SlashRequireGuild on command group level is enough
        await new Show(ctx, (DiscordMember) user, n).RunAsync();
    }

    [SlashCommand("edit", "Edits a quote.")]
    public async Task SlashEdit(InteractionContext ctx,
        [Option("user", "User to edit quote of")]
        DiscordUser user,
        [Option("n", "Number of the quote to edit")]
        long n)
    {
        await new Edit(ctx, (DiscordMember) user, n).RunAsync();
    }

    [SlashCommand("delete", "Deletes a quote.")]
    public async Task SlashDelete(InteractionContext ctx,
        [Option("user", "User to delete quote of")]
        DiscordUser user,
        [Option("n", "Number of the quote to delete")]
        long n)
    {
        await new Delete(ctx, (DiscordMember) user, n).RunAsync();
    }

    [SlashCommand("list", "Lists all quotes from given user.")]
    public async Task SlashList(InteractionContext ctx,
        [Option("user", "User to list quotes of")]
        DiscordUser user)
    {
        // TODO check if SlashRequireGuild on command group level is enough
        await new List(ctx, (DiscordMember) user).RunAsync();
    }

    [SlashCommand("random", "Shows a random quote.")]
    [SlashRequireBotPermissions(Permissions.SendMessages | Permissions.EmbedLinks)]
    public async Task SlashRandom(InteractionContext ctx)
    {
        await new Random(ctx).RunAsync();
    }

    [SlashCommand("search", "Search through quotes containing query.")]
    public async Task SlashSearch(InteractionContext ctx,
        [Option("query", "Query to search for")]
        string query)
    {
        await new Search(ctx, query).RunAsync();
    }
}