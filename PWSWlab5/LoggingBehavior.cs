namespace PWSW5;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> HandleAsync(
        TRequest request,
        Func<Task<TResponse>> next,
        CancellationToken ct)
    {
        var name = typeof(TRequest).Name;
        Console.WriteLine($"[Pipeline] >>> {name}");
        var sw = System.Diagnostics.Stopwatch.StartNew();
        var result = await next();
        sw.Stop();
        Console.WriteLine($"[Pipeline] <<< {name} ({sw.ElapsedMilliseconds} ms)");
        return result;
    }
}
