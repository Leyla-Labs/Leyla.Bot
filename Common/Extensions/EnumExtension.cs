using System.Reflection;

namespace Common.Extensions;

public static class EnumExtension
{
    /// <summary>
    ///     A generic extension method that aids in reflecting
    ///     and retrieving any attribute that is applied to an `Enum`.
    ///     https://stackoverflow.com/a/25109103
    /// </summary>
    public static TAttribute? GetAttribute<TAttribute>(this Enum enumValue)
        where TAttribute : Attribute
    {
        return ((object) enumValue).GetAttribute<TAttribute>();
    }

    public static TAttribute? GetAttribute<TAttribute>(this object enumValue)
        where TAttribute : Attribute
    {
        var member = enumValue.ToString();
        if (member == null)
        {
            throw new NullReferenceException(nameof(member));
        }

        return enumValue.GetType()
            .GetMember(member)
            .First()
            .GetCustomAttribute<TAttribute>();
    }
}