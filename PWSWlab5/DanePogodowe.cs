using System.Text.Json.Serialization;

namespace PWSW5;

public record DanePogodowe(
    [property: JsonPropertyName("city")] string Miasto,
    [property: JsonPropertyName("temp_c")] double TemperaturaCelsjusz,
    [property: JsonPropertyName("humidity_pct")] int WilgotnoscProcent,
    [property: JsonPropertyName("condition")] string Warunki);
