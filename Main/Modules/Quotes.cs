using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Main.Commands.Quotes;

namespace Main.Modules;

[SlashCommandGroup("Quote", "Description TODO")]
[SlashRequireGuild]
public class Quotes : ApplicationCommandModule
{
    [ContextMenu(ApplicationCommandType.MessageContextMenu, "Add Quote")]
    public async Task MenuAddQuote(ContextMenuContext ctx)
    {
        await new AddQuote(ctx).RunAsync();
    }

    [SlashCommand("show", "Shows a specific quote.")]
    [SlashRequireBotPermissions(Permissions.SendMessages | Permissions.EmbedLinks)]
    public async Task SlashShowQuote(InteractionContext ctx,
        [Option("user", "User to show quote of")]
        DiscordUser user,
        [Option("n", "Number of the quote to show")]
        long n)
    {
        // TODO check if SlashRequireGuild on command group level is enough
        await new ShowQuote(ctx, (DiscordMember) user, n).RunAsync();
    }

    [SlashCommand("edit", "Edits a quote.")]
    public async Task SlashEditQuote(InteractionContext ctx,
        [Option("user", "User to edit quote of")]
        DiscordUser user,
        [Option("n", "Number of the quote to edit")]
        long n)
    {
        await new EditQuote(ctx, (DiscordMember) user, n).RunAsync();
    }

    [SlashCommand("delete", "Deletes a quote.")]
    public async Task SlashDeleteQuote(InteractionContext ctx,
        [Option("user", "User to delete quote of")]
        DiscordUser user,
        [Option("n", "Number of the quote to delete")]
        long n)
    {
        await new DeleteQuote(ctx, (DiscordMember) user, n).RunAsync();
    }

    [SlashCommand("list", "Lists all quotes from given user.")]
    public async Task SlashListQuotes(InteractionContext ctx,
        [Option("user", "User to list quotes of")]
        DiscordUser user)
    {
        // TODO check if SlashRequireGuild on command group level is enough
        await new ListQuotes(ctx, (DiscordMember) user).RunAsync();
    }

    [SlashCommand("random", "Shows a random quote.")]
    [SlashRequireBotPermissions(Permissions.SendMessages | Permissions.EmbedLinks)]
    public async Task SlashRandomQuote(InteractionContext ctx)
    {
        await new RandomQuote(ctx).RunAsync();
    }

    [SlashCommand("search", "Search through quotes containing query.")]
    public async Task SlashSearchQuotes(InteractionContext ctx,
        [Option("query", "Query to search for")]
        string query)
    {
        await new SearchQuotes(ctx, query).RunAsync();
    }
}