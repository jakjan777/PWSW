namespace Challenge1;

public static class LoggingMiddleware
{
    public static Func<BuildDelegate, BuildDelegate> Create(string name) =>
        next => async ctx =>
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            ctx.Log.Add($"[{name}] Start");
            await next(ctx);
            sw.Stop();
            ctx.Log.Add($"[{name}] Koniec ({sw.ElapsedMilliseconds} ms)");
        };
}
