using System.Collections.Concurrent;

namespace ApiGateway;

public class RateLimitMiddleware(RequestDelegate nastepny, int maxZapytan = 10, int oknoSekund = 60)
{
    private readonly ConcurrentDictionary<string, List<DateTime>> _zapytania = new();

    public async Task InvokeAsync(HttpContext kontekst)
    {
        var klucz = kontekst.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var listaZapytan = _zapytania.GetOrAdd(klucz, _ => []);

        lock (listaZapytan)
        {
            listaZapytan.RemoveAll(d => DateTime.UtcNow - d > TimeSpan.FromSeconds(oknoSekund));
            if (listaZapytan.Count >= maxZapytan)
            {
                kontekst.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                return;
            }

            listaZapytan.Add(DateTime.UtcNow);
        }

        await nastepny(kontekst);
    }
}
