using Common.Classes;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Main.Enums;
using Main.Handler.ConfigurationOptionActionHandlers;

namespace Main.Handler;

internal sealed class ConfigurationOptionActionSelectedHandler : InteractionHandler
{
    private readonly string _action;
    private readonly string _optionId;

    public ConfigurationOptionActionSelectedHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e,
        string action, string optionId) :
        base(sender, e)
    {
        _optionId = optionId;
        _action = action;
    }

    public override async Task RunAsync()
    {
        var action = Enum.Parse<ConfigurationAction>(_action);

        switch (action)
        {
            case ConfigurationAction.Edit:
                await new ActionEditHandler(Sender, EventArgs, _optionId).RunAsync();
                break;
            case ConfigurationAction.Reset:
                await new ActionResetHandler(Sender, EventArgs, _optionId).RunAsync();
                break;
            case ConfigurationAction.Delete:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}