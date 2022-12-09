using Common.Classes;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Main.Helper;
using NetStone;

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
        var (profile, id) =
            await FfxivHelper.SearchAndGetCharacterDataAsync(Ctx, _name, _server, "ffxivCharacterClaim", true,
                async x =>
                {
                    var stone = await LodestoneClient.GetClientAsync();
                    return (await stone.GetCharacter(x.ToString()), x);
                });

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (profile == null)
        {
            // profile is null if no result or character selection shown - both handled within previous method
            return;
        }

        var name = profile.Name;
        var (status, code) =
            await FfxivHelper.CreateClaimIfNotExistAsync(Ctx.User.Id, id, profile.Bio);
        var embed = FfxivHelper.CreateCharacterClaimStatusEmbed(status, name, code);

        await Ctx.EditResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
    }
}