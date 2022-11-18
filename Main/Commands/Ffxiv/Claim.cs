using Common.Classes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Main.Helper;

namespace Main.Commands.Ffxiv;

public class Claim : SlashCommand
{
    private readonly string _name;
    private readonly string? _server;

    public Claim(InteractionContext ctx, string name, string? server) : base(ctx)
    {
        _name = name;
        _server = server;
    }

    public override async Task RunAsync()
    {
        var profile =
            await FfxivHelper.SearchAndGetCharacterProfileExtended(Ctx, _name, _server, "ffxivCharacterClaim", true);

        if (profile == null)
        {
            // profile is null if no result or character selection shown - both handled within previous method
            return;
        }

        var name = profile.Character.Name;
        var (status, code) =
            await FfxivHelper.CreateClaimIfNotExist(Ctx.User.Id, profile.Character.Id, profile.Character.Bio);
        var embed = FfxivHelper.CreateCharacterClaimStatusEmbed(status, name, code);

        await Ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
    }
}