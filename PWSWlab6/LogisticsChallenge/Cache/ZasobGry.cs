namespace LogisticsChallenge.Cache;

public abstract class ZasobGry : IComparable<ZasobGry>
{
    public string Nazwa { get; init; } = "";
    public long RozmiarBajtow { get; init; }
    public DateTime DataZaladowania { get; init; } = DateTime.Now;

    public int CompareTo(ZasobGry? other) =>
        other is null ? 1 : RozmiarBajtow.CompareTo(other.RozmiarBajtow);
}
