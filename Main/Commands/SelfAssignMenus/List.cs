using System.Text;
using Common.Classes;
using Common.Db.Models;
using Common.Extensions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;

namespace Main.Commands.SelfAssignMenus;

internal sealed class List : SlashCommand
{
    public List(InteractionContext ctx) : base(ctx)
    {
    }

    public override async Task RunAsync()
    {
        var menus = await GetSelfAssignMenusAsync();

        if (!menus.Any())
        {
            await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddErrorEmbed("No Self Assign Menus",
                "There are no self assign menus yet. You can create the first using `/menu create`.").AsEphemeral());
            return;
        }

        var embed = GetEmbed(menus);
        await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
    }

    #region Instance methods

    private async Task<List<SelfAssignMenu>> GetSelfAssignMenusAsync()
    {
        return await DbCtx.SelfAssignMenus.Where(x =>
                x.GuildId == Ctx.Guild.Id)
            .Include(x => x.SelfAssignMenuDiscordEntityAssignments)
            .ToListAsync();
    }

    #endregion

    #region Static methods

    private static DiscordEmbed GetEmbed(IReadOnlyCollection<SelfAssignMenu> menus)
    {
        var embed = new DiscordEmbedBuilder();
        var c = menus.Count;
        embed.WithTitle($"{c} Self Assign {(c == 1 ? "Menu" : "Menus")}");

        var menuStrings = menus.Select(GetMenuString);
        embed.WithDescription(string.Join($"{Environment.NewLine}{Environment.NewLine}", menuStrings));
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }

    private static string GetMenuString(SelfAssignMenu m)
    {
        var sb = new StringBuilder();
        sb.Append($"**{m.Title}**");
        if (!string.IsNullOrWhiteSpace(m.Description))
        {
            sb.Append($"{Environment.NewLine}{m.Description}");
        }

        sb.Append(Environment.NewLine);

        var c = m.SelfAssignMenuDiscordEntityAssignments.Count;
        sb.Append($"{c} {(c == 1 ? "Role" : "Roles")}");
        return sb.ToString();
    }

    #endregion
}