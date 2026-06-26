namespace LogisticsAnalysis.Cache;

public class Cache<T> where T : ZasobGry
{
    private readonly Dictionary<string, T> _elementy = [];
    private readonly int _maksymalnyRozmiar;

    public Cache(int maksymalnyRozmiar = 100) =>
        _maksymalnyRozmiar = maksymalnyRozmiar;

    public void Dodaj(T zasob)
    {
        if (_elementy.Count >= _maksymalnyRozmiar)
        {
            var najstarszy = _elementy
                .OrderBy(kv => kv.Value.DataZaladowania)
                .First().Key;
            _elementy.Remove(najstarszy);
        }
        _elementy[zasob.Nazwa] = zasob;
    }

    public T? Pobierz(string nazwa) =>
        _elementy.GetValueOrDefault(nazwa);

    public List<T> PobierzPosortowane() =>
        _elementy.Values.OrderBy(z => z).ToList();
}
