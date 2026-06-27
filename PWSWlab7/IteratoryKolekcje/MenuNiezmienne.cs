using System.Collections.Immutable;

namespace IteratoryKolekcje;

public class MenuNiezmienne
{
    public ImmutableArray<Danie> Pozycje { get; }

    public MenuNiezmienne(IEnumerable<Danie> pozycje)
    {
        Pozycje = pozycje.ToImmutableArray();
    }

    public static MenuNiezmienne Polacz(List<Danie> bazowe, List<Danie> specjalne)
    {
        List<Danie> pelneMenu =
        [
            .. bazowe,
            .. specjalne,
            new("Espresso", 8.00m, "napoj", ["kawa"])
        ];
        return new MenuNiezmienne(pelneMenu);
    }

    public int LiczbaPozycji => Pozycje.Length;
}
