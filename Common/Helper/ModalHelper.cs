namespace Common.Helper;

public static class ModalHelper
{
    public static string GetModalName(ulong userId, string modalName, string[] additionalInfo)
    {
        return GetModalName(userId.ToString(), modalName, additionalInfo);
    }

    public static string GetModalName(string userId, string modalName, string[] additionalInfo)
    {
        return $"{userId}-{modalName}-{string.Join("-", additionalInfo)}";
    }

    public static string GetModalName(ulong userId, string modalName)
    {
        return $"{userId}-{modalName}";
    }
}