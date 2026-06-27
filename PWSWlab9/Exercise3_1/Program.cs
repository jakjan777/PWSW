using Exercise3_1.Middleware;
using Exercise3_1.Pipeline;

Console.WriteLine("=== Cwiczenie 3.1: Middleware Pipeline i potok CI/CD ===\n");

Console.WriteLine("--- Potok CI/CD: scenariusz sukcesu ---");
var pipeline = new BuildPipeline()
    .AddStep(new RestoreStep())
    .AddStep(new BuildStep())
    .AddStep(new PublishStep());

var ctxOk = new BuildContext
{
    ProjectPath = "Lab09_App",
    Configuration = "Release"
};
await pipeline.ExecuteAsync(ctxOk);
WypiszLog(ctxOk);

Console.WriteLine("\n--- Potok CI/CD: short-circuiting (brak sciezki) ---");
var ctxFail = new BuildContext
{
    ProjectPath = "",
    Configuration = "Release"
};
await pipeline.ExecuteAsync(ctxFail);
WypiszLog(ctxFail);

Console.WriteLine("\n--- CorrelationIdMiddleware (ASP.NET Core) ---");
await DemonstrujMiddlewareAsync();

Console.WriteLine("\n=== Koniec cwiczenia 3.1 ===");

static void WypiszLog(BuildContext ctx)
{
    foreach (var wpis in ctx.Log)
        Console.WriteLine($"  {wpis}");
    Console.WriteLine($"  Sukces: {ctx.Success}, output: {ctx.OutputPath ?? "brak"}");
    if (ctx.ErrorMessage is not null)
        Console.WriteLine($"  Blad: {ctx.ErrorMessage}");
}

static async Task DemonstrujMiddlewareAsync()
{
    var builder = WebApplication.CreateBuilder();
    builder.WebHost.UseUrls("http://127.0.0.1:0");
    builder.Logging.SetMinimumLevel(LogLevel.Information);

    var app = builder.Build();
    app.UseCorrelationId();
    app.MapGet("/api/ping", (HttpContext ctx) =>
        Results.Ok(new
        {
            CorrelationId = ctx.Items[CorrelationIdMiddleware.ItemKey]?.ToString()
        }));

    await app.StartAsync();
    var baseUrl = app.Urls.First();

    using var client = new HttpClient();
    client.DefaultRequestHeaders.Add(CorrelationIdMiddleware.HeaderName, "test-correlation-42");

    var response = await client.GetAsync($"{baseUrl}/api/ping");
    var body = await response.Content.ReadAsStringAsync();
    var header = response.Headers.GetValues(CorrelationIdMiddleware.HeaderName).First();

    Console.WriteLine($"  Status: {(int)response.StatusCode}");
    Console.WriteLine($"  Naglowek {CorrelationIdMiddleware.HeaderName}: {header}");
    Console.WriteLine($"  Odpowiedz: {body}");

    await app.StopAsync();
}
