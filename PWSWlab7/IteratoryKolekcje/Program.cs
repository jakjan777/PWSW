using IteratoryKolekcje;

Console.WriteLine("Fibonacci (pierwsze 10):");
Console.WriteLine(string.Join(", ", Sekwencje.Fibonacci().Take(10)));

Console.WriteLine();
Console.WriteLine("Zakres(0, 20, 3):");
Console.WriteLine(string.Join(", ", Sekwencje.Zakres(0, 20, 3)));

Console.WriteLine();
Console.WriteLine("Collatz(27):");
Console.WriteLine(string.Join(" -> ", Sekwencje.Collatz(27)));

Console.WriteLine();
Console.WriteLine("Stronicowany odczyt (Take 12 z 35 rekordow):");
var baza = new BazaDanych(35);
var rekordy = baza.StronicowanyOdczyt(10).Take(12).ToList();
Console.WriteLine($"Pobrano {rekordy.Count} rekordow: {rekordy.First().Nazwa} .. {rekordy.Last().Nazwa}");

Console.WriteLine();
Console.WriteLine($"Opis kuchni wloskiej: {KatalogKuchni.OpisyKuchni["wloska"]}");
Console.WriteLine($"Alergeny w katalogu: {KatalogKuchni.Alergeny.Count}");

List<Danie> bazowe = [new("Margherita", 32m, "pizza", ["ser", "pomidor"])];
List<Danie> specjalne = [new("Diavola", 38m, "pizza", ["ser", "salami"])];
var menuStale = MenuNiezmienne.Polacz(bazowe, specjalne);
Console.WriteLine($"Menu niezmienne (spread): {menuStale.LiczbaPozycji} pozycji");

var restauracje = new List<Restauracja>
{
    new("Trattoria Bella", "wloska")
    {
        Menu = [.. bazowe, .. specjalne],
        Oceny =
        [
            new("Anna", 5, DateTime.Today.AddDays(-2)),
            new("Piotr", 4, DateTime.Today.AddDays(-5))
        ]
    },
    new("Sushi Zen", "japonska")
    {
        Menu = [new("Maki set", 45m, "sushi", ["ryba", "ryz"])],
        Oceny = [new("Kasia", 5, DateTime.Today)]
    },
    new("Chata Polska", "polska")
    {
        Menu = [new("Pierogi ruskie", 28m, "dania-glowne", ["gluten"])],
        Oceny =
        [
            new("Tomek", 3, DateTime.Today.AddDays(-1)),
            new("Ola", 4, DateTime.Today.AddDays(-3))
        ]
    },
    new("Ramen House", "japonska")
    {
        Menu = [new("Tonkotsu", 42m, "ramen", ["gluten", "jajka"])],
        Oceny = [new("Marek", 5, DateTime.Today), new("Ewa", 5, DateTime.Today.AddDays(-1))]
    },
    new("Pizzeria Napoli", "wloska")
    {
        Menu = [new("Quattro Formaggi", 36m, "pizza", ["laktoza"])],
        Oceny = [new("Jan", 4, DateTime.Today)]
    }
};

Console.WriteLine();
Console.WriteLine("Top 5 restauracji:");
foreach (var (r, wynik) in SilnikRankingu.TopN(restauracje))
    Console.WriteLine($"  {r.Nazwa} ({r.TypKuchni}) — wynik: {wynik:F1}, srednia ocen: {r.SredniaOcen:F1}");
