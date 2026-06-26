namespace LogisticsAnalysis.Models;

public record Przesylka(
    string NumerTracking,
    string MiastoNadania,
    string MiastoDostawy,
    int KurierId,
    DateTime DataNadania,
    DateTime? DataDostawy,
    decimal Waga,
    string Kategoria) // "Standardowa", "Ekspresowa", "Gabaryt"
{
    public int? CzasDostawyDni =>
        DataDostawy.HasValue
            ? (DataDostawy.Value - DataNadania).Days
            : null;
}
