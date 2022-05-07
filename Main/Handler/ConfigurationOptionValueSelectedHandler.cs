using Common.Classes;
using Db.Enums;
using Db.Helper;
using Db.Statics;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Main.Handler;

public class ConfigurationOptionValueSelectedHandler : InteractionHandler
{
    private readonly ulong _optionId;

    public ConfigurationOptionValueSelectedHandler(DiscordClient sender, ComponentInteractionCreateEventArgs e,
        ulong optionId) : base(sender, e)
    {
        _optionId = optionId;
    }

    public override async Task RunAsync()
    {
        var option = ConfigOptions.Instance.Get(_optionId);
        var value = EventArgs.Values[0];

        switch (option.Type)
        {
            case ConfigType.Boolean:
                var valueBool = value.Equals("1");
                await ConfigHelper.Instance.Set(option, EventArgs.Guild.Id, valueBool);
                break;
            case ConfigType.Role:
            case ConfigType.Channel:
                var valueUlong = Convert.ToUInt64(value);
                await ConfigHelper.Instance.Set(option, EventArgs.Guild.Id, valueUlong);
                break;
            case ConfigType.String:
            case ConfigType.Int:
            case ConfigType.Char:
            default:
                throw new ArgumentOutOfRangeException();
        }

        await EventArgs.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
    }
}