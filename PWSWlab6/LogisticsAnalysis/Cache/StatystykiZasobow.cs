using System.Numerics;

namespace LogisticsAnalysis.Cache;

public static class StatystykiZasobow
{
    public static T ObliczSrednia<T>(IEnumerable<T> wartosci)
        where T : INumber<T>
    {
        T suma = T.Zero;
        T licznik = T.Zero;
        foreach (var w in wartosci)
        {
            suma += w;
            licznik += T.One;
        }
        return licznik == T.Zero ? T.Zero : suma / licznik;
    }
}
