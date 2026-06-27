namespace Exercise1_1;

public static class ProcesorLogow
{
    private static WpisLogu? ParsujLinie(string linia)
    {
        var czesci = linia.Split('|', 4);
        if (czesci.Length != 4) return null;
        if (!DateTime.TryParse(czesci[0], out var czas)) return null;
        return new WpisLogu(czas, czesci[1], czesci[2], czesci[3]);
    }

    public static IEnumerable<WpisLogu> CzytajLogi(string sciezka)
    {
        using var reader = new StreamReader(sciezka);
        string? linia;
        while ((linia = reader.ReadLine()) is not null)
        {
            var wpis = ParsujLinie(linia);
            if (wpis is not null) yield return wpis;
        }
    }

    public static async IAsyncEnumerable<WpisLogu> CzytajLogiAsync(string sciezka)
    {
        using var reader = new StreamReader(sciezka);
        string? linia;
        while ((linia = await reader.ReadLineAsync()) is not null)
        {
            var wpis = ParsujLinie(linia);
            if (wpis is not null) yield return wpis;
        }
    }
}
