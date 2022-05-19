namespace Common.Classes;

public class DisplayString
{
    private readonly string? _description;
    public readonly string Name;

    public DisplayString(string name, string description)
    {
        Name = name;
        _description = description;
    }

    public DisplayString(string name)
    {
        Name = name;
    }

    public string Description => _description ?? string.Empty;
}