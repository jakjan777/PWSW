using LogisticsChallenge.Cache;
using LogisticsChallenge.Models;
using LogisticsChallenge.Reports;

var kurierzy = new List<Kurier>
{
    new(1, "Jan", "Kowalski", "Polnoc"),
    new(2, "Anna", "Nowak", "Poludnie"),
    new(3, "Piotr", "Wisniewski", "Polnoc"),
    new(4, "Maria", "Dabrowska", "Zachod"),
};

var przesylki = new List<Przesylka>
{
    new("TRK001", "Bydgoszcz", "Warszawa", 1, new DateTime(2026, 3, 1), new DateTime(2026, 3, 3), 2.5m, "Standardowa"),
    new("TRK002", "Bydgoszcz", "Warszawa", 3, new DateTime(2026, 3, 2), new DateTime(2026, 3, 4), 1.8m, "Ekspresowa"),
    new("TRK003", "Bydgoszcz", "Gdansk", 1, new DateTime(2026, 3, 1), new DateTime(2026, 3, 2), 3.2m, "Standardowa"),
    new("TRK004", "Torun", "Poznan", 2, new DateTime(2026, 3, 3), new DateTime(2026, 3, 6), 5.0m, "Gabaryt"),
    new("TRK005", "Torun", "Poznan", 2, new DateTime(2026, 3, 4), new DateTime(2026, 3, 7), 4.1m, "Standardowa"),
    new("TRK006", "Wroclaw", "Krakow", 4, new DateTime(2026, 3, 1), new DateTime(2026, 3, 10), 12.0m, "Gabaryt"),
    new("TRK007", "Bydgoszcz", "Warszawa", 1, new DateTime(2026, 3, 5), null, 2.0m, "Ekspresowa"),
    new("TRK008", "Bydgoszcz", "Gdansk", 3, new DateTime(2026, 3, 6), new DateTime(2026, 3, 7), 1.5m, "Standardowa"),
    new("TRK009", "Wroclaw", "Krakow", 4, new DateTime(2026, 3, 2), new DateTime(2026, 3, 4), 6.5m, "Ekspresowa"),
    new("TRK010", "Torun", "Poznan", 2, new DateTime(2026, 3, 8), new DateTime(2026, 3, 11), 3.8m, "Standardowa"),
};

Console.WriteLine("=== Wyzwanie 1: Analizator wydajnosci kurierskiej ===\n");

var rankingRegionow = przesylki
    .Join(kurierzy, p => p.KurierId, k => k.Id,
        (p, k) => new { p.CzasDostawyDni, k.Region })
    .Where(x => x.CzasDostawyDni.HasValue)
    .GroupBy(x => x.Region)
    .Select(g => new
    {
        Region = g.Key,
        SredniCzas = g.Average(x => x.CzasDostawyDni!.Value),
        LiczbaPrzesylek = g.Count(),
    })
    .OrderBy(r => r.Region)
    .ToList();

Console.WriteLine("Sredni czas dostawy per region:");
foreach (var region in rankingRegionow)
{
    Console.WriteLine($"  {region.Region}: {region.SredniCzas:F1} dni ({region.LiczbaPrzesylek} przesylek)");
}

double sredniaCalosc = przesylki
    .Where(p => p.CzasDostawyDni.HasValue)
    .Average(p => p.CzasDostawyDni!.Value);

var rankingKurierow = przesylki
    .Join(kurierzy, p => p.KurierId, k => k.Id,
        (p, k) => new { p.CzasDostawyDni, Kurier = $"{k.Imie} {k.Nazwisko}", k.Region })
    .Where(x => x.CzasDostawyDni.HasValue)
    .GroupBy(x => new { x.Kurier, x.Region })
    .Select(g => new
    {
        g.Key.Kurier,
        g.Key.Region,
        SredniCzas = g.Average(x => x.CzasDostawyDni!.Value),
        LiczbaPrzesylek = g.Count(),
    })
    .OrderBy(k => k.SredniCzas)
    .ToList();

var anomalie = rankingKurierow
    .Where(k => k.SredniCzas > sredniaCalosc * 1.5)
    .ToList();

Console.WriteLine($"\nSrednia calego systemu: {sredniaCalosc:F1} dni (prog anomalii: {sredniaCalosc * 1.5:F1} dni)");
Console.WriteLine("Anomalie (kurierzy > 150% sredniej systemowej):");
if (anomalie.Count == 0)
{
    Console.WriteLine("  Brak anomalii.");
}
else
{
    foreach (var k in anomalie)
    {
        Console.WriteLine($"  {k.Kurier} ({k.Region}): {k.SredniCzas:F1} dni");
    }
}

var cache = new Cache<WynikRegionu>();
foreach (var region in rankingRegionow)
{
    cache.Dodaj(new WynikRegionu
    {
        Nazwa = region.Region,
        Region = region.Region,
        SredniCzasDni = region.SredniCzas,
        LiczbaPrzesylek = region.LiczbaPrzesylek,
        RozmiarBajtow = region.LiczbaPrzesylek * 64,
    });
}

Console.WriteLine("\nCache wynikow regionow (TryGet):");
foreach (var region in rankingRegionow)
{
    if (cache.TryGet(region.Region, out var wynik))
    {
        Console.WriteLine($"  {wynik!.Region}: {wynik.SredniCzasDni:F1} dni, {wynik.LiczbaPrzesylek} przesylek");
    }
}

var anomalieSet = anomalie.Select(a => a.Kurier).ToHashSet();
var podsumowanie =
    $"Przeanalizowano {przesylki.Count(p => p.CzasDostawyDni.HasValue)} dostarczonych przesylek. " +
    $"Sredni czas systemu: {sredniaCalosc:F1} dni. " +
    $"Liczba regionow: {rankingRegionow.Count}. " +
    $"Wykryto {anomalie.Count} anomalii kurierskich (>150% sredniej).";

if (anomalie.Count > 0)
{
    podsumowanie += " Kurierzy z anomalia: " +
                    string.Join(", ", anomalie.Select(a => $"{a.Kurier} ({a.SredniCzas:F1} dni)")) + ".";
}

var raport = new FluentReportBuilder()
    .WithTitle("Raport wydajnosci kurierskiej")
    .WithAuthor("System analityczny")
    .AddText("Podsumowanie", podsumowanie)
    .AddTable(
        "Dane per kurier",
        ["Kurier", "Region", "Sredni czas", "Przesylki", "Anomalia"],
        rankingKurierow.Select(k => new[]
        {
            k.Kurier,
            k.Region,
            $"{k.SredniCzas:F1} dni",
            k.LiczbaPrzesylek.ToString(),
            anomalieSet.Contains(k.Kurier) ? "TAK" : "NIE",
        }))
    .InFormat(ReportFormat.Html)
    .WithMetadata("SredniaSystemu", sredniaCalosc.ToString("F1"))
    .Build();

Console.WriteLine("\n=== Raport wynikowy ===");
Console.WriteLine($"Tytul: {raport.Title}");
Console.WriteLine($"Autor: {raport.Author}");
Console.WriteLine($"Format: {raport.Format}");
Console.WriteLine($"Wygenerowano: {raport.GeneratedAt:yyyy-MM-dd HH:mm:ss}");

foreach (var sekcja in raport.Sections)
{
    Console.WriteLine($"\n[{sekcja.Type}] {sekcja.Heading}");
    Console.WriteLine(sekcja.Content);
}
