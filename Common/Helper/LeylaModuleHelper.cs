using Common.Enums;

namespace Common.Helper;

public static class LeylaModuleHelper
{
    public static IDictionary<LeylaModule, ulong> LeylaModules { get; } = Initialize();

    public static IEnumerable<ulong> UserIds => LeylaModules.Values;

    private static IDictionary<LeylaModule, ulong> Initialize()
    {
        var modulesStr = Environment.GetEnvironmentVariable("MODULES") ?? throw new NullReferenceException();
        modulesStr = modulesStr.ToUpperInvariant();
        var modules = modulesStr.Split(";");

        var dict = new Dictionary<LeylaModule, ulong>();

        foreach (var moduleStr in modules)
        {
            var idStr = Environment.GetEnvironmentVariable($"ID_{moduleStr}");

            var (module, id) = moduleStr switch
            {
                "MAIN" when ulong.TryParse(idStr, out var result) => (LeylaModule.Main, result),
                "LOGS" when ulong.TryParse(idStr, out var result) => (LeylaModule.Logs, result),
                "SPAM" when ulong.TryParse(idStr, out var result) => (LeylaModule.Spam, result),
                _ => throw new ArgumentOutOfRangeException(nameof(idStr), idStr)
            };
            dict.Add(module, id);
        }

        return dict;
    }
}