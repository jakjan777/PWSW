using LogisticsAnalysis.Cache;
using LogisticsAnalysis.Generics;
using LogisticsAnalysis.Models;

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

Console.WriteLine("=== Analiza danych logistycznych ===\n");

// Najczestsze trasy (grupowanie po parze miast)
var popularneTrasy = przesylki
    .GroupBy(p => (p.MiastoNadania, p.MiastoDostawy))
    .Select(g => new
    {
        Trasa = $"{g.Key.MiastoNadania} -> {g.Key.MiastoDostawy}",
        Liczba = g.Count(),
        SredniaWaga = g.Average(p => p.Waga),
    })
    .OrderByDescending(t => t.Liczba);

Console.WriteLine("Popularne trasy:");
foreach (var trasa in popularneTrasy)
{
    Console.WriteLine($"  {trasa.Trasa}: {trasa.Liczba} przesylek, srednia waga {trasa.SredniaWaga:F2} kg");
}

// Ranking kurierow polaczony z danymi (Join)
var rankingKurierow = przesylki
    .Join(kurierzy, p => p.KurierId, k => k.Id,
        (p, k) => new { p.CzasDostawyDni, Kurier = $"{k.Imie} {k.Nazwisko}" })
    .Where(x => x.CzasDostawyDni.HasValue)
    .GroupBy(x => x.Kurier)
    .Select(g => new
    {
        Kurier = g.Key,
        LiczbaPrzesylek = g.Count(),
        SredniCzas = g.Average(x => x.CzasDostawyDni!.Value),
    })
    .OrderBy(k => k.SredniCzas);

Console.WriteLine("\nRanking kurierow (najszybsi na gorze):");
foreach (var kurier in rankingKurierow)
{
    Console.WriteLine($"  {kurier.Kurier}: {kurier.LiczbaPrzesylek} przesylek, sredni czas {kurier.SredniCzas:F1} dni");
}

// Wykrywanie anomalii (czas > 2 x srednia)
var dostarczone = przesylki.Where(p => p.CzasDostawyDni.HasValue).ToList();
double srednia = dostarczone.Average(p => p.CzasDostawyDni!.Value);
var anomalie = dostarczone
    .Where(p => p.CzasDostawyDni!.Value > srednia * 2)
    .ToList();

Console.WriteLine($"\nSredni czas dostawy (dostarczone): {srednia:F1} dni");
Console.WriteLine($"Anomalie (czas > 2x srednia = {srednia * 2:F1} dni):");
if (anomalie.Count == 0)
{
    Console.WriteLine("  Brak anomalii.");
}
else
{
    foreach (var p in anomalie)
    {
        Console.WriteLine($"  {p.NumerTracking}: {p.MiastoNadania} -> {p.MiastoDostawy}, {p.CzasDostawyDni} dni");
    }
}

Console.WriteLine("\n=== Generyczny cache zasobow ===\n");

var cache = new Cache<TeksturaGry>(maksymalnyRozmiar: 3);

cache.Dodaj(new TeksturaGry
{
    Nazwa = "trawa.png",
    RozmiarBajtow = 2048,
    Szerokosc = 256,
    Wysokosc = 256,
    DataZaladowania = DateTime.Now.AddMinutes(-30),
});
cache.Dodaj(new TeksturaGry
{
    Nazwa = "skala.png",
    RozmiarBajtow = 8192,
    Szerokosc = 512,
    Wysokosc = 512,
    DataZaladowania = DateTime.Now.AddMinutes(-20),
});
cache.Dodaj(new TeksturaGry
{
    Nazwa = "woda.png",
    RozmiarBajtow = 4096,
    Szerokosc = 512,
    Wysokosc = 256,
    DataZaladowania = DateTime.Now.AddMinutes(-10),
});
cache.Dodaj(new TeksturaGry
{
    Nazwa = "piasek.png",
    RozmiarBajtow = 3072,
    Szerokosc = 256,
    Wysokosc = 256,
});

Console.WriteLine("Cache po dodaniu 4. zasobu (limit 3, usuniety najstarszy):");
Console.WriteLine($"  trawa.png w cache: {cache.Pobierz("trawa.png") is not null}");
Console.WriteLine($"  piasek.png w cache: {cache.Pobierz("piasek.png") is not null}");

Console.WriteLine("\nZasoby posortowane po rozmiarze:");
foreach (var z in cache.PobierzPosortowane())
{
    Console.WriteLine($"  {z.Nazwa}: {z.RozmiarBajtow} B");
}

var rozmiary = cache.PobierzPosortowane().Select(z => z.RozmiarBajtow);
Console.WriteLine($"\nSredni rozmiar (long): {StatystykiZasobow.ObliczSrednia(rozmiary)} B");
Console.WriteLine($"Srednia z int: {StatystykiZasobow.ObliczSrednia([10, 20, 30])}");
Console.WriteLine($"Srednia z decimal: {StatystykiZasobow.ObliczSrednia([1.5m, 2.5m, 3.5m])}");

Console.WriteLine("\nKowariancja i kontrawariancja:");
IProducent<TeksturaGry> producentTekstur = new ProducentTekstur();
IProducent<ZasobGry> producentOgolny = producentTekstur;
var utworzony = producentOgolny.Utworz();
Console.WriteLine($"  Kowariancja out T: utworzono {utworzony.Nazwa}");

IKonsument<ZasobGry> konsumentOgolny = new KonsumentZasobow();
IKonsument<TeksturaGry> konsumentTekstur = konsumentOgolny;
konsumentTekstur.Przetworz(new TeksturaGry
{
    Nazwa = "metal.png",
    RozmiarBajtow = 6144,
    Szerokosc = 256,
    Wysokosc = 256,
});
