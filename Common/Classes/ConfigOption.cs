using Common.Enums;
using Common.GuildConfig;
using Common.Interfaces;

namespace Common.Classes;

public class ConfigOption : IIdentifiable
{
    private readonly int _configOptionCategoryId;

    public ConfigOption(int id,
        int sortId,
        DisplayString displayString,
        int categoryId,
        ConfigType type,
        bool nullable,
        string? defaultValue,
        LeylaModule module)
    {
        Id = id;
        SortId = sortId;
        Name = displayString.Name;
        Description = displayString.Description;
        _configOptionCategoryId = categoryId;
        ConfigType = type;
        Nullable = nullable;
        DefaultValue = defaultValue;
        Module = module;
        Nullable = nullable;
    }

    public ConfigOption(int id,
        int sortId,
        DisplayString displayString,
        int categoryId,
        Type enumType,
        bool nullable,
        string? defaultValue,
        LeylaModule module) : this(id, sortId, displayString, categoryId, ConfigType.Enum, nullable, defaultValue,
        module)
    {
        if (!enumType.IsEnum)
        {
            throw new ArgumentException("Type must be an enum", nameof(enumType));
        }

        EnumType = enumType;
    }

    public ConfigType ConfigType { get; }
    public string? DefaultValue { get; }
    public Type? EnumType { get; }
    public LeylaModule Module { get; }
    public bool Nullable { get; }
    public ConfigOptionCategory ConfigOptionCategory => ConfigOptionCategories.Instance.Get(_configOptionCategoryId);
    public int Id { get; }
    public string Name { get; }
    public string Description { get; }
    public int SortId { get; }
}