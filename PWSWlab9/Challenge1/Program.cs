using Challenge1;

Console.WriteLine("=== Wyzwanie 1: Use, SigningStep, LoggingMiddleware ===\n");

Console.WriteLine("--- Scenariusz sukcesu (artefakt msix) ---");
var pipeline = new BuildPipeline()
    .Use(LoggingMiddleware.Create("CI"))
    .Use(SigningStep.Create());

var ctxOk = new BuildContext
{
    ProjectPath = "MyApp",
    Artifacts = new() { ["msix"] = "app.msix" }
};
await pipeline.Run(ctxOk);
WypiszLog(ctxOk);

Console.WriteLine("\n--- Scenariusz short-circuit (brak msix) ---");
var ctxFail = new BuildContext
{
    ProjectPath = "MyApp",
    Artifacts = []
};
await pipeline.Run(ctxFail);
WypiszLog(ctxFail);

Console.WriteLine("\n=== Koniec wyzwania 1 ===");

static void WypiszLog(BuildContext ctx)
{
    foreach (var wpis in ctx.Log)
        Console.WriteLine($"  {wpis}");
    Console.WriteLine($"  Sukces: {ctx.Success}");
}
