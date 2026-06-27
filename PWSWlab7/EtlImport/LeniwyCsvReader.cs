namespace EtlImport;

public static class LeniwyCsvReader
{
    public static IEnumerable<Dictionary<string, string>> CzytajWiersze(string sciezka)
    {
        using var reader = new StreamReader(sciezka);
        var naglowek = reader.ReadLine();
        if (naglowek is null) yield break;

        var headers = naglowek.Split(';');
        string? linia;
        while ((linia = reader.ReadLine()) is not null)
        {
            var values = linia.Split(';');
            var rekord = new Dictionary<string, string>();
            for (int i = 0; i < headers.Length && i < values.Length; i++)
                rekord[headers[i].Trim()] = values[i].Trim();
            yield return rekord;
        }
    }
}
