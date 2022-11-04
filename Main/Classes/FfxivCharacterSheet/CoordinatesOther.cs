using SixLabors.ImageSharp;

namespace Main.Classes.FfxivCharacterSheet;

internal static class CoordinatesOther
{
    public static Point TextTop => new(854, 243);
    
    public static Point FcOrGcTop =>
        new(TextTop.X + (int) decimal.Divide(Values.DimensionsGcFcCrest + Values.GcCrestPadding, 2), TextTop.Y);
    
    public static Point GcBottom => new(532, 320);

    public static Point JobIcon => new(214, 19);
}