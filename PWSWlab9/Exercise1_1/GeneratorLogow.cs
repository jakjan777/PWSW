namespace Exercise1_1;

public static class GeneratorLogow
{
    private static readonly string[] Poziomy =
        ["INFO", "INFO", "INFO", "WARN", "WARN", "ERROR", "FATAL"];

    private static readonly string[] Zrodla =
        ["WebApi", "Database", "Auth", "Cache", "Scheduler"];

    public static async Task GenerujPlikAsync(string sciezka, int liczbaWpisow)
    {
        var random = new Random(42);
        await using var writer = new StreamWriter(sciezka);
        var data = new DateTime(2026, 1, 1);

        for (int i = 0; i < liczbaWpisow; i++)
        {
            data = data.AddSeconds(random.Next(1, 60));
            string poziom = Poziomy[random.Next(Poziomy.Length)];
            string zrodlo = Zrodla[random.Next(Zrodla.Length)];
            await writer.WriteLineAsync(
                $"{data:yyyy-MM-dd HH:mm:ss}|{poziom}|{zrodlo}" +
                $"|Operacja #{i} z modulu {zrodlo}");
        }
    }
}
