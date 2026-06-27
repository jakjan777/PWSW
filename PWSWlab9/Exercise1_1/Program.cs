using Exercise1_1;

var katalog = AppContext.BaseDirectory;
var sciezkaLogu = Path.Combine(katalog, "server.log");
var sciezkaCsv = Path.Combine(katalog, "raport.csv");

Console.WriteLine("=== Cwiczenie 1.1: Strumieniowy procesor logu ===\n");

Console.WriteLine("Generowanie pliku testowego (10000 wpisow)...");
await GeneratorLogow.GenerujPlikAsync(sciezkaLogu, 10_000);
Console.WriteLine($"Utworzono: {sciezkaLogu}\n");

var bledy = ProcesorLogow.CzytajLogi(sciezkaLogu)
    .Where(w => w.Poziom is "ERROR" or "FATAL")
    .Take(10)
    .ToList();

Console.WriteLine($"Pierwsze {bledy.Count} bledow (ERROR/FATAL):");
foreach (var b in bledy)
    Console.WriteLine($"  [{b.Czas:HH:mm:ss}] {b.Poziom} | {b.Zrodlo} | {b.Wiadomosc}");

var statystyki = ProcesorLogow.CzytajLogi(sciezkaLogu)
    .GroupBy(w => w.Zrodlo)
    .Select(g => new
    {
        Zrodlo = g.Key,
        Info = g.Count(w => w.Poziom == "INFO"),
        Error = g.Count(w => w.Poziom == "ERROR"),
    })
    .OrderByDescending(s => s.Error)
    .ToList();

await using var csv = new StreamWriter(sciezkaCsv);
await csv.WriteLineAsync("Zrodlo,Info,Error");
foreach (var s in statystyki)
    await csv.WriteLineAsync($"{s.Zrodlo},{s.Info},{s.Error}");

Console.WriteLine($"\nEksport statystyk do: {sciezkaCsv}");
Console.WriteLine("Top 3 zrodla wg liczby ERROR:");
foreach (var s in statystyki.Take(3))
    Console.WriteLine($"  {s.Zrodlo}: INFO={s.Info}, ERROR={s.Error}");

Console.WriteLine("\n--- Wersja asynchroniczna (IAsyncEnumerable) ---");
int licznikAsync = 0;
await foreach (var wpis in ProcesorLogow.CzytajLogiAsync(sciezkaLogu))
{
    if (wpis.Poziom == "FATAL")
    {
        Console.WriteLine($"  FATAL: {wpis.Wiadomosc}");
        licznikAsync++;
        if (licznikAsync >= 3) break;
    }
}

Console.WriteLine("\n=== Koniec cwiczenia 1.1 ===");
