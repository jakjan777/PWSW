namespace PWSW5;

public static class LogEntryExtensions
{
    public static IEnumerable<LogEntry> ZKategorii(
        this IEnumerable<LogEntry> logi, string kategoria) =>
        logi.Where(l => l.Category == kategoria);

    public static IEnumerable<LogEntry> ZOkresu(
        this IEnumerable<LogEntry> logi,
        DateTime od, DateTime doData) =>
        logi.Where(l => l.Timestamp >= od && l.Timestamp <= doData);

    public static string JakoRaport(this IEnumerable<LogEntry> logi)
    {
        var lista = logi.ToList();
        if (lista.Count == 0) return "Brak wpisow.";

        var kategorie = string.Join(", ", lista.Select(l => l.Category).Distinct());
        return $"Wpisy: {lista.Count}, kategorie: {kategorie}, " +
               $"od: {lista.Min(l => l.Timestamp):dd.MM HH:mm}, " +
               $"do: {lista.Max(l => l.Timestamp):dd.MM HH:mm}";
    }
}
