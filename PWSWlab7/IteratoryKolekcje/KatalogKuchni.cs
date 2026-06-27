using System.Collections.Frozen;

namespace IteratoryKolekcje;

public static class KatalogKuchni
{
    public static readonly FrozenDictionary<string, string> OpisyKuchni =
        new Dictionary<string, string>
        {
            ["wloska"] = "Makarony, pizze, risotto",
            ["japonska"] = "Sushi, ramen, tempura",
            ["polska"] = "Pierogi, bigos, zurek",
        }.ToFrozenDictionary();

    public static readonly FrozenSet<string> Alergeny =
        new HashSet<string>
        {
            "gluten", "laktoza", "orzechy", "jajka", "ryba"
        }.ToFrozenSet();
}
