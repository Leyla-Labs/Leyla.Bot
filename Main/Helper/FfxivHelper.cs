using Common.Db;
using Common.Db.Models;
using Common.Extensions;
using Common.Helper;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Main.Enums;
using Microsoft.EntityFrameworkCore;
using xivapi_cs;
using xivapi_cs.Enums;
using xivapi_cs.ViewModels.CharacterProfile;
using xivapi_cs.ViewModels.CharacterSearch;

namespace Main.Helper;

internal static class FfxivHelper
{
    public static async Task<T?> SearchAndGetCharacterData<T>(InteractionContext ctx,
        string name, string? server, string selectId, bool asEphemeral, Func<int, Task<T?>> func)
    {
        HomeWorld? homeWorld = null;

        if (server != null)
        {
            var succ = Enum.TryParse(typeof(HomeWorld), server, true, out var result);

            if (succ && result != null)
            {
                homeWorld = (HomeWorld) result;
            }
            else
            {
                await ctx.CreateResponseAsync(
                    new DiscordInteractionResponseBuilder().AddErrorEmbed("Home world not found."));
                return default;
            }
        }

        await ctx.DeferAsync(asEphemeral);

        var characterSearch = homeWorld != null
            ? await new XivApiClient().CharacterSearch(name, homeWorld.Value)
            : await new XivApiClient().CharacterSearch(name);

        if (characterSearch == null || !characterSearch.Results.Any())
        {
            var errorMsg = $"{name}{(server != null ? $" ({server})" : string.Empty)} not found.";
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddErrorEmbed(errorMsg));
            return default;
        }

        if (characterSearch.Results.Length > 1)
        {
            var characterSelect = GetCharacterSelect(ctx, characterSearch, selectId);
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddComponents(characterSelect));
            return default;
        }

        var characterData = await func(characterSearch.Results.First().Id);

        if (characterData == null)
        {
            await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddErrorEmbed("Could not get character data."));
        }

        return characterData;
    }


    /// <summary>
    ///     Checks if a claim for the given character already exists and creates it if not.
    /// </summary>
    /// <param name="userId">User to create the claim for.</param>
    /// <param name="characterId">FFXIV Lodestone character ID.</param>
    /// <param name="bio">Lodestone bio for given character.</param>
    /// <returns>Tuple of claim status and code. Code is only returned if claim belongs to given userId.</returns>
    public static async Task<(CharacterClaimStatus status, string? code)> CreateClaimIfNotExist(ulong userId,
        int characterId, string? bio)
    {
        var code = LodestoneBioToCode(bio);

        await using var context = new DatabaseContext();

        var characterClaim = await context.CharacterClaims.FirstOrDefaultAsync(x => x.CharacterId == characterId);

        if (characterClaim != null)
        {
            // If a claim for this character already exists

            if (characterClaim.UserId == userId)
            {
                // If the existing claim belongs to the given user

                if (characterClaim.Confirmed)
                {
                    // If claim already confirmed
                    return (CharacterClaimStatus.ClaimAlreadyConfirmed, characterClaim.Code);
                }

                if (characterClaim.Code.Equals(code))
                {
                    characterClaim.Confirmed = true;
                    await context.SaveChangesAsync();
                    return (CharacterClaimStatus.ClaimNewlyConfirmed, code);
                }

                return (CharacterClaimStatus.ClaimExists, characterClaim.Code);
            }

            if (!characterClaim.Confirmed && characterClaim.ValidUntil < DateTime.UtcNow)
            {
                // claim has expired, delete
                context.CharacterClaims.Remove(characterClaim);
            }
            else
            {
                // claim is still valid
                return characterClaim.Confirmed
                    ? (CharacterClaimStatus.ClaimConfirmedForDifferentUser, null)
                    : (CharacterClaimStatus.ClaimExistsForDifferentUser, null);
            }
        }

        var cObj = new CharacterClaim
        {
            UserId = userId,
            CharacterId = characterId,
            Code = code,
            ValidUntil = DateTime.UtcNow.AddDays(7)
        };

        await context.CharacterClaims.AddAsync(cObj);
        await context.SaveChangesAsync();
        return (CharacterClaimStatus.ClaimCreated, cObj.Code);
    }

    public static DiscordEmbed CreateCharacterClaimStatusEmbed(CharacterClaimStatus status, string name, string? code)
    {
        return status switch
        {
            CharacterClaimStatus.ClaimCreated => CreateClaimCreatedEmbed(name, code!),
            CharacterClaimStatus.ClaimExists => CreateClaimExistsEmbed(name, code!),
            CharacterClaimStatus.ClaimNewlyConfirmed => CreateClaimNewlyConfirmedEmbed(name),
            CharacterClaimStatus.ClaimAlreadyConfirmed => CreateClaimAlreadyConfirmedEmbed(name),
            CharacterClaimStatus.ClaimExistsForDifferentUser => CreateClaimExistsForDifferentUserEmbed(name),
            CharacterClaimStatus.ClaimConfirmedForDifferentUser => CreateClaimConfirmedForDifferentUser(name),
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
    }

    public static async Task RespondToSlashWithSheet(InteractionContext ctx, CharacterProfileExtended profile)
    {
        var helper = await CharacterSheetHelper.Create(profile);
        var stream = await helper.GetCharacterSheet();
        var fileName = helper.GetFileName();
        await ctx.EditResponseAsync(new DiscordWebhookBuilder().AddFile(fileName, stream, true));
    }

    private static string LodestoneBioToCode(string? bio)
    {
        if (bio == null || bio.Length < 2 || !bio[..2].Equals("L-"))
        {
            return GenerateNewCode();
        }

        return Guid.TryParse(bio[2..], out _) ? bio : GenerateNewCode();
    }

    private static string GenerateNewCode()
    {
        return $"L-{Guid.NewGuid()}";
    }

    private static DiscordSelectComponent GetCharacterSelect(BaseContext ctx, CharacterSearch result, string selectId)
    {
        var name = ModalHelper.GetModalName(ctx.User.Id, selectId);
        var options = result.Results.Take(25).Select(x =>
            new DiscordSelectComponentOption(x.Name, x.Id.ToString(), x.HomeWorldDetails.HomeWorld.ToString()));
        var t = result.Pagination.ResultsTotal;
        var suffix = t > 25
            ? $" (Showing 25/{t} results)./"
            : $" ({t} results).";
        return new DiscordSelectComponent(name, $"Select character {suffix}", options, minOptions: 1, maxOptions: 1);
    }

    #region CharacterClaimStatus embed

    private static DiscordEmbed CreateClaimCreatedEmbed(string name, string code)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle($"Claim created: {name}");
        embed.WithDescription(
            "To confirm this claim, please copy and paste the code below onto your Lodestone profile. " +
            "Afterwards, please run the command again to get an updated status.");
        embed.AddField("Code", $"`{code}`");
        // TODO add video tutorial
        return embed.Build();
    }

    private static DiscordEmbed CreateClaimExistsEmbed(string name, string code)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle($"Claim already exists: {name}");
        embed.WithDescription(
            "To confirm this claim, please copy and paste the code below onto your Lodestone profile. " +
            "If you have already added the code to your profile, please wait a few hours and try again.");
        embed.AddField("Code", $"`{code}`");
        // TODO add video tutorial
        return embed.Build();
    }

    private static DiscordEmbed CreateClaimNewlyConfirmedEmbed(string name)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle($"Claim confirmed: {name}");
        embed.WithDescription(
            "You have now claimed your character! " +
            "From this point, you can quickly fetch your character sheet using `/ffxiv character me`. " +
            "You can now delete the code from your Lodestone profile.");
        return embed.Build();
    }

    private static DiscordEmbed CreateClaimAlreadyConfirmedEmbed(string name)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle($"Claim already confirmed: {name}");
        embed.WithDescription("This character has already been confirmed to be you.");
        return embed.Build();
    }

    private static DiscordEmbed CreateClaimExistsForDifferentUserEmbed(string name)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle($"Claim already exists: {name}");
        embed.WithDescription("An unconfirmed claim for this character already exists for a different user. " +
                              "Unconfirmed claims expire after three days, so you may try again in a few days.");
        return embed.Build();
    }

    private static DiscordEmbed CreateClaimConfirmedForDifferentUser(string name)
    {
        var embed = new DiscordEmbedBuilder();
        embed.WithTitle($"Claim already exists: {name}");
        embed.WithDescription("This character has already been claimed by a different user. " +
                              "You cannot claim this character.");
        return embed.Build();
    }

    #endregion
}