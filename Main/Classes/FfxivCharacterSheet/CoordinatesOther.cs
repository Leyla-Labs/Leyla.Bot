using SixLabors.ImageSharp;

namespace Main.Classes.FfxivCharacterSheet;

internal static class CoordinatesOther
{
    public static Point TextTop => new(854, 243);

    public static Point FcOrGcTop =>
        new(TextTop.X + (int) decimal.Divide(Values.DimensionsGcFcCrest + Values.GcCrestPadding, 2), TextTop.Y);

    public static Point GcBottom => new(540, 320);

    public static Point JobIcon => new(214, 19);
    public static Point ActiveJobLevelBackground => new(72, 127);
    public static Point ActiveJobLevelText => new(72, 124);
    public static Point AttributesPrimary => new(853, 443);
    public static Point AttributesSecondary => new(853, 503);
    public static Point HomeWorld => new(1114, 344);
    public static Point ItemLevel => new(613, 338);
    public static Point Minions => new(772, ItemLevel.Y);
    public static Point Mounts => new(874, ItemLevel.Y);
}