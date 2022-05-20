using Common.Classes;
using Common.Enums;
using Common.Helper;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Main.Commands.UserLogs;

public class AddUserLog : ContextMenuCommand
{
    public AddUserLog(ContextMenuContext ctx) : base(ctx)
    {
    }

    public override async Task RunAsync()
    {
        var logTypeSelect = GetUserLogTypeSelect();
        await Ctx.CreateResponseAsync(
            new DiscordInteractionResponseBuilder().AddComponents(logTypeSelect).AsEphemeral());
    }

    #region Instance methods

    private DiscordSelectComponent GetUserLogTypeSelect()
    {
        var options = ((UserLogType[]) Enum.GetValues(typeof(UserLogType))).Select(type =>
            new DiscordSelectComponentOption(type.GetName(), ((int) type).ToString())).ToList();
        var customId = ModalHelper.GetModalName(Ctx.User.Id, "userLogType", new[] {Ctx.TargetUser.Id.ToString()});
        return new DiscordSelectComponent(customId, "Select log type", options, minOptions: 1,
            maxOptions: 1);
    }

    #endregion
}