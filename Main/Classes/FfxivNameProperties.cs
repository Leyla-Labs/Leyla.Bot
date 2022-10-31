using xivapi_cs.ViewModels.CharacterProfile;

namespace Main.Classes;

internal class FfxivNameProperties
{
    public FfxivNameProperties(CharacterExtended character)
    {
        var x = 854;

        if (string.IsNullOrEmpty(character.Title.Name))
        {
            Name = new FontProperties(x, 100, 46);
        }
        else
        {
            if (character.TitleTop)
            {
                Name = new FontProperties(x, 120, 46);
                Title = new FontProperties(x, 77, 30);
            }
            else
            {
                Name = new FontProperties(x, 85, 46);
                Title = new FontProperties(x, 128, 30);
            }
        }
    }

    public FontProperties Name { get; }
    public FontProperties? Title { get; }
}

internal class FontProperties
{
    public FontProperties(int x, int y, int size)
    {
        X = x;
        Y = y;
        Size = size;
    }

    public int X { get; }
    public int Y { get; }
    public int Size { get; }
}