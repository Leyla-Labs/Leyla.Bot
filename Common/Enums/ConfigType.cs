using System.ComponentModel.DataAnnotations;

namespace Common.Enums;

public enum ConfigType
{
    [Display(Name = "String", Description = "Text (eg. `Hello there`)")]
    String = 1,

    [Display(Name = "Boolean", Description = "`Yes` / `No`")]
    Boolean = 2,

    [Display(Name = "Integer", Description = "Whole number (eg. `7`)")]
    Int = 3,

    [Display(Name = "Char", Description = "Single character (eg. `z`)")]
    Char = 4,

    [Display(Name = "Role", Description = "Any Discord Role from your server")]
    Role = 5,

    [Display(Name = "Channel", Description = "Any Discord channel from your server")]
    Channel = 6,

    [Display(Name = "Decimal", Description = "A decimal number (eg. `8.54` / `7`)")]
    Decimal = 7,

    [Display(Name = "Enum", Description = "Selection from a set of specific values")]
    Enum = 8
}