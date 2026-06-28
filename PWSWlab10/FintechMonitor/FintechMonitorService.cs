using System.Threading.Channels;
using FintechMonitor.Resilience;

namespace FintechMonitor;

public enum ServiceHealth { Healthy, Degraded, Critical }

public record ServiceAlert(
    string ServiceName,
    ServiceHealth Health,
    string Message,
    DateTime Timestamp);

public record MonitorProgress(
    int Cycle, string CurrentService, ServiceHealth Status);

public class FintechMonitorService
{
    private readonly ManualCircuitBreaker _bankBreaker = new(3, TimeSpan.FromSeconds(10));
    private readonly Channel<ServiceAlert> _alertChannel =
        Channel.CreateBounded<ServiceAlert>(new BoundedChannelOptions(50)
        {
            FullMode = BoundedChannelFullMode.Wait
        });

    private readonly Random _rng = new(42);

    public async Task RunAsync(
        int maxCycles,
        IProgress<MonitorProgress> progress,
        CancellationToken ct)
    {
        var consumer = Task.Run(() => ConsumeAlertsAsync(ct), ct);

        for (int cycle = 1; cycle <= maxCycles; cycle++)
        {
            using var cycleCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cycleCts.CancelAfter(TimeSpan.FromSeconds(5));

            var bank = CheckBankApiAsync(cycleCts.Token);
            var exchange = CheckExchangeAsync(cycleCts.Token);
            var fraud = CheckFraudAsync(cycleCts.Token);

            var allChecks = Task.WhenAll(bank, exchange, fraud);
            var firstCritical = WhenFirstCritical(bank, exchange, fraud);

            var completed = await Task.WhenAny(allChecks, firstCritical);

            if (completed == firstCritical)
            {
                var critical = await firstCritical;
                await _alertChannel.Writer.WriteAsync(critical, ct);
                Console.WriteLine(
                    $"[ALARM] {critical.ServiceName}: {critical.Message}");
            }

            ServiceAlert[] results;
            try
            {
                results = await allChecks;
            }
            catch (OperationCanceledException) when (cycleCts.IsCancellationRequested)
            {
                Console.WriteLine($"[TIMEOUT] Cykl {cycle} przekroczyl 5 sekund");
                await Task.Delay(1000, ct);
                continue;
            }

            foreach (var alert in results)
            {
                progress.Report(new MonitorProgress(
                    cycle, alert.ServiceName, alert.Health));
                await _alertChannel.Writer.WriteAsync(alert, ct);
            }

            await Task.Delay(1000, ct);
        }

        _alertChannel.Writer.Complete();
        await consumer;
    }

    private async Task ConsumeAlertsAsync(CancellationToken ct)
    {
        await foreach (var alert in _alertChannel.Reader.ReadAllAsync(ct))
        {
            Console.WriteLine(
                $"[Kanal] {alert.Timestamp:HH:mm:ss} " +
                $"{alert.ServiceName,-15} {alert.Health,-10} {alert.Message}");
        }
    }

    private static Task<ServiceAlert> WhenFirstCritical(
        params Task<ServiceAlert>[] tasks)
    {
        var tcs = new TaskCompletionSource<ServiceAlert>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        foreach (var task in tasks)
        {
            task.ContinueWith(t =>
            {
                if (t.IsCompletedSuccessfully &&
                    t.Result.Health == ServiceHealth.Critical)
                    tcs.TrySetResult(t.Result);
            }, TaskScheduler.Default);
        }

        return tcs.Task;
    }

    private async Task<ServiceAlert> CheckBankApiAsync(CancellationToken ct)
    {
        try
        {
            return await _bankBreaker.ExecuteAsync(async () =>
            {
                await Task.Delay(_rng.Next(200, 800), ct);
                if (_rng.Next(100) < 55)
                    throw new HttpRequestException("Bank API timeout");

                var health = _rng.Next(100) switch
                {
                    < 50 => ServiceHealth.Healthy,
                    < 75 => ServiceHealth.Degraded,
                    _ => ServiceHealth.Critical
                };
                return BuildAlert("Bank API", health);
            });
        }
        catch (CircuitBrokenException ex)
        {
            return new ServiceAlert(
                "Bank API", ServiceHealth.Degraded, ex.Message, DateTime.UtcNow);
        }
        catch (HttpRequestException ex)
        {
            return new ServiceAlert(
                "Bank API", ServiceHealth.Degraded, ex.Message, DateTime.UtcNow);
        }
    }

    private async Task<ServiceAlert> CheckExchangeAsync(CancellationToken ct)
    {
        await Task.Delay(_rng.Next(100, 400), ct);
        var health = _rng.Next(100) switch
        {
            < 80 => ServiceHealth.Healthy,
            _ => ServiceHealth.Degraded
        };
        return BuildAlert("Exchange API", health);
    }

    private async Task<ServiceAlert> CheckFraudAsync(CancellationToken ct)
    {
        await Task.Delay(_rng.Next(150, 600), ct);
        var health = _rng.Next(100) switch
        {
            < 70 => ServiceHealth.Healthy,
            < 90 => ServiceHealth.Degraded,
            _ => ServiceHealth.Critical
        };
        return BuildAlert("Fraud API", health);
    }

    private static ServiceAlert BuildAlert(string name, ServiceHealth health) =>
        new(name, health, health switch
        {
            ServiceHealth.Healthy => "OK",
            ServiceHealth.Degraded => "Degraded",
            _ => "CRITICAL ALERT"
        }, DateTime.UtcNow);
}
