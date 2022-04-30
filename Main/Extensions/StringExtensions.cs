using System.Text.RegularExpressions;

namespace Main.Extensions;

public static class StringExtensions
{
    public static string StripHtml(this string input)
    {
        return Regex.Replace(input, "<[a-zA-Z/].*?>", string.Empty);
    }

    public static Stream GenerateStream(this string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}