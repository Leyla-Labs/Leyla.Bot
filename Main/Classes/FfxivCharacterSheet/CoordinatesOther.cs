using SixLabors.ImageSharp;

namespace Main.Classes.FfxivCharacterSheet;

internal static class CoordinatesOther
{
    public static Point GrandCompanyTop =>
        new(854 + (int) decimal.Divide(Values.DimensionsGcCrest + Values.GcCrestPadding, 2), 243);

    public static Point GrandCompanyBottom => new(532, 320);

    public static Point JobIcon => new(214, 19);
}