namespace PWSW5;

public class PluginCallHandler : IRequestHandler<PluginCallRequest, PluginCallResponse>
{
    public async Task<PluginCallResponse> HandleAsync(
        PluginCallRequest request, CancellationToken ct)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        Console.WriteLine(
            $"[Handler] Plugin '{request.PluginName}' -> '{request.Query}'");
        await Task.Delay(50, ct);
        sw.Stop();
        return new PluginCallResponse(
            true,
            $"Wynik z {request.PluginName}: {request.Query}",
            sw.ElapsedMilliseconds);
    }
}
