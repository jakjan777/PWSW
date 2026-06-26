namespace LogisticsChallenge.Models;

public record Przesylka(
    string NumerTracking,
    string MiastoNadania,
    string MiastoDostawy,
    int KurierId,
    DateTime DataNadania,
    DateTime? DataDostarczenia,
    decimal Waga,
    string Typ)
{
    public int? CzasDostawyDni =>
        DataDostarczenia.HasValue
            ? (DataDostarczenia.Value - DataNadania).Days
            : null;
}
