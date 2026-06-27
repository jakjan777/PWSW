using ApiGateway;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddTransient<AutoryzacjaApiMiddleware>();

var app = builder.Build();

app.Use(async (kontekst, nastepny) =>
{
    var stoper = System.Diagnostics.Stopwatch.StartNew();
    Console.WriteLine($">> {kontekst.Request.Method} {kontekst.Request.Path}{kontekst.Request.QueryString}");
    await nastepny(kontekst);
    stoper.Stop();
    Console.WriteLine($"<< {kontekst.Response.StatusCode} w {stoper.ElapsedMilliseconds} ms");
});

app.UseMiddleware<RateLimitMiddleware>(5, 60);
app.UseMiddleware<AutoryzacjaApiMiddleware>();

app.MapWhen(
    kontekst => kontekst.Request.Query.ContainsKey("debug")
                && kontekst.Request.Query["debug"] == "1",
    galaz =>
    {
        galaz.Run(async kontekst =>
        {
            await kontekst.Response.WriteAsJsonAsync(new
            {
                Status = "DEBUG",
                Sciezka = kontekst.Request.Path.Value,
                Metoda = kontekst.Request.Method,
                Komunikat = "Galaz MapWhen aktywna dla parametru debug=1"
            });
        });
    });

app.MapGet("/api/dane", () => new { Status = "OK", Dane = "Tajne dane" });

app.Run();
