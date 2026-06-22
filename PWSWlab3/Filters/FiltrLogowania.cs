namespace PWSWlab3.Filters;

public class FiltrLogowania : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var request = context.HttpContext.Request;

        Console.WriteLine($">> {request.Method} {request.Path}");

        var result = await next(context);

        stopwatch.Stop();
        Console.WriteLine($"<< Odpowiedz: {stopwatch.ElapsedMilliseconds} ms");

        return result;
    }
}
