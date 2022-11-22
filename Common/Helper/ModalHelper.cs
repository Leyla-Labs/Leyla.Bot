namespace Common.Helper;

public static class ModalHelper
{
    public static string GetModalName(ulong userId, string modalName, string[] additionalInfo)
    {
        return $"{userId}_{modalName}_{string.Join("_", additionalInfo)}";
    }

    public static string GetModalName(ulong userId, string modalName)
    {
        return $"{userId}_{modalName}";
    }
}