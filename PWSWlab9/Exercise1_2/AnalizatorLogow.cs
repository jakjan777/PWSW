using System.Buffers;
using System.Text.RegularExpressions;

namespace Exercise1_2;

public partial class AnalizatorLogow
{
    [GeneratedRegex(
        @"(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})",
        RegexOptions.Compiled)]
    private static partial Regex RegexIp();

    [GeneratedRegex(
        @"(?<typ>Brute-force|SQL injection|Port scan|DDoS)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex RegexAtak();

    private static readonly SearchValues<char> _znakiSpecjalne =
        SearchValues.Create("'\";<>(){}[]&|");

    public static List<string> WyodrebnijAdresyIp(string tekst)
    {
        var wynik = new List<string>();
        foreach (Match m in RegexIp().Matches(tekst))
            wynik.Add(m.Groups[1].Value);
        return wynik;
    }

    public static List<string> WyodrebnijTypyAtakow(string tekst)
    {
        var wynik = new List<string>();
        foreach (Match m in RegexAtak().Matches(tekst))
            wynik.Add(m.Groups["typ"].Value);
        return wynik;
    }

    public static int ZliczZnakiSpecjalne(ReadOnlySpan<char> tekst)
    {
        int licznik = 0;
        var reszta = tekst;
        while (true)
        {
            int idx = reszta.IndexOfAny(_znakiSpecjalne);
            if (idx < 0) break;
            licznik++;
            reszta = reszta[(idx + 1)..];
        }
        return licznik;
    }
}
