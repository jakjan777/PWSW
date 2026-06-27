namespace IteratoryKolekcje;

public record Rekord(int Id, string Nazwa, DateTime DataUtworzenia);

public class BazaDanych(int liczbaRekordow)
{
    private readonly List<Rekord> _dane = Enumerable.Range(1, liczbaRekordow)
        .Select(i => new Rekord(i, $"Element-{i:D4}", DateTime.Today.AddDays(-i)))
        .ToList();

    private List<Rekord> PobierzStrone(int offset, int limit)
    {
        Console.WriteLine($"[DB] Pobieranie rekordow {offset}..{offset + limit}");
        return _dane.Skip(offset).Take(limit).ToList();
    }

    public IEnumerable<Rekord> StronicowanyOdczyt(int rozmiarStrony = 10)
    {
        int offset = 0;
        while (true)
        {
            var strona = PobierzStrone(offset, rozmiarStrony);
            if (strona.Count == 0) yield break;
            foreach (var rekord in strona)
                yield return rekord;
            offset += rozmiarStrony;
        }
    }
}
