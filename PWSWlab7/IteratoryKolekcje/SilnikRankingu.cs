namespace IteratoryKolekcje;

public static class SilnikRankingu
{
    public static double ObliczWynik(Restauracja restauracja)
    {
        double wynik = restauracja.SredniaOcen * 10;
        if (KatalogKuchni.OpisyKuchni.ContainsKey(restauracja.TypKuchni))
            wynik += 5;
        wynik += restauracja.Menu.Count * 0.5;
        return wynik;
    }

    public static List<(Restauracja Restauracja, double Wynik)> TopN(
        IEnumerable<Restauracja> restauracje,
        int limit = 5)
    {
        var kolejka = new PriorityQueue<Restauracja, double>();
        foreach (var r in restauracje)
            kolejka.Enqueue(r, -ObliczWynik(r));

        List<(Restauracja, double)> ranking = [];
        while (kolejka.Count > 0 && ranking.Count < limit)
        {
            kolejka.TryDequeue(out var r, out double ujemnyWynik);
            if (r is not null)
                ranking.Add((r, -ujemnyWynik));
        }

        return ranking;
    }
}
