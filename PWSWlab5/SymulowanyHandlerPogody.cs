using System.Net;
using System.Text;
using System.Text.Json;

namespace PWSW5;

internal sealed class SymulowanyHandlerPogody : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var query = request.RequestUri?.Query ?? "";
        var miasto = "Nieznane";
        foreach (var part in query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var kv = part.Split('=', 2);
            if (kv.Length == 2 && kv[0] == "miasto")
                miasto = Uri.UnescapeDataString(kv[1]);
        }

        var rng = new Random(miasto.GetHashCode());
        var dane = new DanePogodowe(
            Miasto: miasto,
            TemperaturaCelsjusz: rng.Next(-5, 25),
            WilgotnoscProcent: rng.Next(40, 90),
            Warunki: rng.Next(2) == 0 ? "Slonecznie" : "Pochmurno");

        var json = JsonSerializer.Serialize(new
        {
            city = dane.Miasto,
            temp_c = dane.TemperaturaCelsjusz,
            humidity_pct = dane.WilgotnoscProcent,
            condition = dane.Warunki
        });

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        return Task.FromResult(response);
    }
}
