namespace SecurityScanner;

public static class SubsystemMonitor
{
    private static readonly Random _rng = new(42);

    public static async Task<SubsystemReport> CheckAsync(
        string name, int delayMs, CancellationToken ct = default)
    {
        var start = DateTime.Now;
        await Task.Delay(delayMs, ct);

        var status = _rng.Next(100) switch
        {
            < 60 => SubsystemStatus.Healthy,
            < 85 => SubsystemStatus.Warning,
            _ => SubsystemStatus.Failure
        };

        string details = status switch
        {
            SubsystemStatus.Healthy => "Parametry w normie",
            SubsystemStatus.Warning => "Drobne odchylenia",
            SubsystemStatus.Failure => "KRYTYCZNY BLAD!",
            _ => "Nieznany"
        };

        return new SubsystemReport(name, status, details,
            DateTime.Now - start);
    }
}
