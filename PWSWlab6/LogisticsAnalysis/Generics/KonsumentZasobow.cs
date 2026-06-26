using LogisticsAnalysis.Cache;

namespace LogisticsAnalysis.Generics;

public class KonsumentZasobow : IKonsument<ZasobGry>
{
    public void Przetworz(ZasobGry element) =>
        Console.WriteLine($"  Przetworzono zasob: {element.Nazwa}, {element.RozmiarBajtow} B");
}
