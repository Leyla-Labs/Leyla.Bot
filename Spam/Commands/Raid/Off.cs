using Common.Classes;
using Common.Helper;
using Common.Strings;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Spam.Commands.Raid;

public class Off : SlashCommand
{
    public Off(InteractionContext ctx) : base(ctx)
    {
    }

    public override async Task RunAsync()
    {
        await ConfigHelper.Instance.Set(Config.Raid.RaidMode.Name, Ctx.Guild.Id, false);
        await Ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder().AddEmbed(GetEmbed()));
    }

    private DiscordEmbed GetEmbed()
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle("Raid Mode Disabled");
        embed.WithDescription("All members going forward can join normally.");
        embed.WithColor(DiscordColor.Blurple);
        return embed.Build();
    }
}