using Common.Classes;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Spam.Helper;

namespace Spam.Handler;

internal sealed class DisableLockdownHandler : InteractionHandler
{
    private readonly string _verificationLevel;

    public DisableLockdownHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e, string verificationLevel)
        : base(sender, e)
    {
        _verificationLevel = verificationLevel;
    }

    public override async Task RunAsync()
    {
        await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
        var verificationLevel = (VerificationLevel) Convert.ToInt32(_verificationLevel);
        RaidHelper.Instance.StopLockdownTimer(EventArgs.Guild.Id);
        await EventArgs.Guild.ModifyAsync(x => x.VerificationLevel = verificationLevel);
        await RaidHelper.SendLockdownDisabledMessage(EventArgs.Guild);
    }
}