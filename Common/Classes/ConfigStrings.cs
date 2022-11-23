namespace Common.Classes;

public class ConfigStrings
{
    public readonly string Description;
    public readonly string Name;
    public readonly string? WikiUrl;

    internal ConfigStrings(string name, string description, string? wikiUrl = null)
    {
        Name = name;
        Description = description;
        WikiUrl = wikiUrl;
    }
}