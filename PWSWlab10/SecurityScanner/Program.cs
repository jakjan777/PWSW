using SecurityScanner;

var tasks = new[]
{
    SubsystemMonitor.CheckAsync("API Gateway", 800),
    SubsystemMonitor.CheckAsync("Baza danych", 600),
    SubsystemMonitor.CheckAsync("Auth Service", 500),
    SubsystemMonitor.CheckAsync("AI Model", 900),
    SubsystemMonitor.CheckAsync("Notification", 700),
};

SubsystemReport[] reports = await Task.WhenAll(tasks);

Console.WriteLine("Raport diagnostyczny:");
foreach (var r in reports)
{
    string icon = r.Status switch
    {
        SubsystemStatus.Healthy => "[ OK ]",
        SubsystemStatus.Warning => "[!!]",
        SubsystemStatus.Failure => "[ XX ]",
        _ => "[??]"
    };
    Console.WriteLine(
        $"{icon} {r.Name,-20} {r.Details} " +
        $"({r.CheckTime.TotalMilliseconds:F0} ms)");
}

Console.WriteLine();
Console.WriteLine("Monitorowanie alarmow (Task.WhenAny):");

var alarmTasks = new[]
{
    SubsystemMonitor.CheckAsync("Firewall", 1200),
    SubsystemMonitor.CheckAsync("IDS", 300),
    SubsystemMonitor.CheckAsync("Logging", 800),
};

var pending = alarmTasks.ToList();
while (pending.Count > 0)
{
    var finished = await Task.WhenAny(pending);
    pending.Remove(finished);
    var alert = await finished;

    if (alert.Status is SubsystemStatus.Warning or SubsystemStatus.Failure)
    {
        string level = alert.Status == SubsystemStatus.Failure ? "KRYTYCZNY" : "OSTRZEZENIE";
        Console.WriteLine(
            $"[{level}] Pierwszy alarm: {alert.Name} - {alert.Details} " +
            $"({alert.CheckTime.TotalMilliseconds:F0} ms)");
        break;
    }

    Console.WriteLine($"[OK] {alert.Name} - brak alarmu ({alert.CheckTime.TotalMilliseconds:F0} ms)");
}

Console.WriteLine();
using var semaphore = new SemaphoreSlim(2);
string[] systems = Enumerable.Range(1, 8)
    .Select(i => $"Modul-{i:D2}").ToArray();

var limited = systems.Select(async name =>
{
    await semaphore.WaitAsync();
    try
    {
        return await SubsystemMonitor.CheckAsync(name, 400);
    }
    finally { semaphore.Release(); }
});

var limitedReports = await Task.WhenAll(limited);
foreach (var r in limitedReports)
{
    string icon = r.Status switch
    {
        SubsystemStatus.Healthy => "[ OK ]",
        SubsystemStatus.Warning => "[!!]",
        SubsystemStatus.Failure => "[ XX ]",
        _ => "[??]"
    };
    Console.WriteLine($"{icon} {r.Name,-12} {r.Details}");
}

Console.WriteLine("Skan ograniczony zakonczony.");

Console.WriteLine();
Console.WriteLine("Pelny skan z timeoutem i postepem:");

using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

var prog = new Progress<ScanProgress>(p =>
    Console.Write(
        $"\r[{new string('#', p.Percent / 5),-20}] {p.Percent}% {p.Step}"));

try
{
    int total = await FullScanRunner.RunFullScanAsync(1000, prog, cts.Token);
    Console.WriteLine($"\nPrzeskanowano {total} elementow.");
}
catch (OperationCanceledException)
{
    Console.WriteLine("\nSKAN ANULOWANY --- przekroczono czas!");
}
