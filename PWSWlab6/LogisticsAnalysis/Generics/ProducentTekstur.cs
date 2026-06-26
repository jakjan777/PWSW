using LogisticsAnalysis.Cache;

namespace LogisticsAnalysis.Generics;

public class ProducentTekstur : IProducent<TeksturaGry>
{
    public TeksturaGry Utworz() => new()
    {
        Nazwa = "domyslna_tekstura",
        RozmiarBajtow = 4096,
        Szerokosc = 512,
        Wysokosc = 512,
    };
}
