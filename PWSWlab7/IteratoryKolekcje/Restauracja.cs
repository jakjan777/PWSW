namespace IteratoryKolekcje;

public record Danie(string Nazwa, decimal Cena, string Kategoria, List<string> Skladniki);

public record Ocena(string Autor, int Gwiazdki, DateTime Data);

public class Restauracja(string nazwa, string typKuchni)
{
    public string Nazwa { get; } = nazwa;
    public string TypKuchni { get; } = typKuchni;
    public List<Danie> Menu { get; init; } = [];
    public List<Ocena> Oceny { get; init; } = [];
    public double SredniaOcen => Oceny.Count > 0
        ? Oceny.Average(o => o.Gwiazdki)
        : 0;
}
