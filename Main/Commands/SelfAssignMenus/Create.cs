using System.Text;
using Common.Classes;
using Common.Db.Models;
using Common.Helper;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Main.Commands.SelfAssignMenus;

public class Create : SlashCommand
{
    private readonly string? _description;
    private readonly string _title;

    public Create(InteractionContext ctx, string title, string? description) : base(ctx)
    {
        _title = title;
        _description = description;
    }

    public override async Task RunAsync()
    {
        await CreateInDatabase();
        var embed = GetEmbed();
        await Ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
    }

    #region Instance methods

    private async Task CreateInDatabase()
    {
        await GuildHelper.CreateIfNotExist(Ctx.Guild.Id);

        await DbCtx.SelfAssignMenus.AddAsync(new SelfAssignMenu
        {
            Title = _title,
            Description = _description,
            GuildId = Ctx.Guild.Id
        });
        await DbCtx.SaveChangesAsync();
    }

    private DiscordEmbed GetEmbed()
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Self Assign Menu Created");

        var sb = new StringBuilder();
        sb.Append($"The self assign menu `{_title}` has been created. ");
        sb.Append("You can change its title or description using /menu rename, ");
        sb.Append("and manage the roles in it using /menu manage.");
        embed.WithDescription(sb.ToString());
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }

    #endregion
}