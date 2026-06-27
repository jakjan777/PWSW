namespace Exercise3_1.Middleware;

public class CorrelationIdMiddleware(
    RequestDelegate next,
    ILogger<CorrelationIdMiddleware> logger)
{
    public const string HeaderName = "X-Correlation-Id";
    public const string ItemKey = "CorrelationId";

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault()
            ?? Guid.NewGuid().ToString("N")[..12];

        context.Items[ItemKey] = correlationId;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        logger.LogInformation(
            "Zadanie {Method} {Path} CorrelationId={CorrelationId}",
            context.Request.Method,
            context.Request.Path,
            correlationId);

        await next(context);
    }
}

public static class CorrelationIdMiddlewareExtensions
{
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder app)
        => app.UseMiddleware<CorrelationIdMiddleware>();
}
