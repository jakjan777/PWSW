namespace PWSW5;

public static class LogExtensions
{
    public static IEnumerable<LogZdarzenia> ZOkresu(
        this IEnumerable<LogZdarzenia> logi,
        DateTime od, DateTime doData) =>
        logi.Where(l => l.Data >= od && l.Data <= doData);

    public static IEnumerable<LogZdarzenia> ZKategorii(
        this IEnumerable<LogZdarzenia> logi, string kategoria) =>
        logi.Where(l => l.Kategoria == kategoria);

    public static IEnumerable<IReadOnlyList<T>> Porcjuj<T>(
        this IEnumerable<T> zrodlo, int rozmiar)
    {
        var porcja = new List<T>(rozmiar);
        foreach (var element in zrodlo)
        {
            porcja.Add(element);
            if (porcja.Count == rozmiar)
            {
                yield return porcja;
                porcja = new List<T>(rozmiar);
            }
        }
        if (porcja.Count > 0) yield return porcja;
    }

    public static string JakoRaport(this IEnumerable<LogZdarzenia> logi)
    {
        var lista = logi.ToList();
        if (lista.Count == 0) return "Brak zdarzen.";
        return $"Zdarzen: {lista.Count}, " +
               $"od: {lista.Min(l => l.Data):dd.MM HH:mm}, " +
               $"do: {lista.Max(l => l.Data):dd.MM HH:mm}";
    }
}
