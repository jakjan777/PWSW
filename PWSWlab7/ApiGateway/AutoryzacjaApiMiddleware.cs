namespace ApiGateway;

public class AutoryzacjaApiMiddleware(ILogger<AutoryzacjaApiMiddleware> logger) : IMiddleware
{
    private const string PoprawnyKlucz = "lab07-secret-key";

    public async Task InvokeAsync(HttpContext kontekst, RequestDelegate nastepny)
    {
        if (!kontekst.Request.Headers.TryGetValue("X-Api-Key", out var klucz)
            || klucz != PoprawnyKlucz)
        {
            logger.LogWarning("Odrzucono zadanie bez poprawnego klucza API: {Path}", kontekst.Request.Path);
            kontekst.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await kontekst.Response.WriteAsync("Brak lub niepoprawny naglowek X-Api-Key.");
            return;
        }

        await nastepny(kontekst);
    }
}
