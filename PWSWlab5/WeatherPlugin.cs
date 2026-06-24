using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace PWSW5;

public class WeatherPlugin : IAgentPlugin
{
    public string Nazwa => "WeatherPlugin";
    public string OpisFunkcji => "Pobieranie danych pogodowych";

    [KernelFunction, Description("Pobiera aktualna pogode dla miasta")]
    public string GetWeather([Description("Nazwa miasta")] string city)
    {
        var rng = new Random(city.GetHashCode());
        var temp = rng.Next(-10, 35);
        var warunki = new[] { "Slonecznie", "Pochmurno", "Deszczowo", "Snieg" };
        return $"Pogoda w {city}: {temp}C, {warunki[rng.Next(warunki.Length)]}";
    }

    [KernelFunction, Description("Pobiera prognoze pogody na 3 dni")]
    public string GetForecast([Description("Nazwa miasta")] string city)
    {
        var rng = new Random(city.GetHashCode() + DateTime.Today.DayOfYear);
        var dni = Enumerable.Range(1, 3).Select(d => $"Dzien {d}: {rng.Next(-10, 35)}C");
        return $"Prognoza dla {city}: {string.Join(", ", dni)}";
    }
}
