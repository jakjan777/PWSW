using FintechMonitor;

var monitor = new FintechMonitorService();

var progress = new Progress<MonitorProgress>(p =>
    Console.Write(
        $"\rCykl {p.Cycle} | {p.CurrentService,-15} | {p.Status}    "));

using var cts = new CancellationTokenSource();

Console.WriteLine("=== Fintech Monitor (Wyzwanie 1) ===");
Console.WriteLine("Task.WhenAll + timeout 5s | Circuit Breaker Bank API");
Console.WriteLine("Task.WhenAny Critical | Channel bounded 50");
Console.WriteLine();

await monitor.RunAsync(5, progress, cts.Token);

Console.WriteLine();
Console.WriteLine("Monitor zakonczony.");
