using Common.Classes;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Spam.Commands.Silence;
using Spam.Enums;

namespace Spam.Modules;

[SlashCommandGroup("Silence", "todo")]
internal sealed class Silence : ApplicationCommandLogModule
{
    [SlashCommand("for", "Silences for the given duration.")]
    [SlashRequireBotPermissions(Permissions.SendMessages | Permissions.EmbedLinks)]
    [SlashRequireGuild]
    public async Task SlashSilenceUntilAsync(InteractionContext ctx,
        [Option("member", "Member to silence")]
        DiscordUser user,
        [Option("n", "How long to silence. Minutes, hours, and days are defined next.")]
        long n,
        [Option("kind", "Silence for minutes, hours, or days")]
        SilenceDurationKind kind)
    {
        await new For(ctx, (DiscordMember) user, n, kind).RunAsync();
    }
}