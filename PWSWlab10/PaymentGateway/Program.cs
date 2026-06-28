using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using PaymentGateway;
using PaymentGateway.Resilience;

Console.WriteLine("=== Reczny Circuit Breaker ===");

var breaker = new ManualCircuitBreaker(3, TimeSpan.FromSeconds(2));
var rng = new Random(42);

for (int i = 1; i <= 6; i++)
{
    try
    {
        var result = await breaker.ExecuteAsync(async () =>
        {
            await Task.Delay(50);
            if (rng.Next(100) < 70)
                throw new HttpRequestException("Blad API banku");
            return $"Platnosc {i} OK";
        });
        Console.WriteLine($"[{breaker.State}] {result}");
    }
    catch (CircuitBrokenException ex)
    {
        Console.WriteLine($"[{breaker.State}] {ex.Message}");
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"[{breaker.State}] Blad: {ex.Message}");
    }
}

Console.WriteLine();
Console.WriteLine("=== AddStandardResilienceHandler ===");

var host = Host.CreateDefaultBuilder()
    .ConfigureServices(services =>
    {
        services.AddHttpClient<ResilientBankClient>(client =>
        {
            client.BaseAddress = new Uri("https://api.bank-external.com");
        })
        .AddStandardResilienceHandler(options =>
        {
            options.CircuitBreaker.FailureRatio = 0.5;
            options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(20);
            options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(10);
            options.CircuitBreaker.MinimumThroughput = 3;

            options.Retry.MaxRetryAttempts = 2;
            options.Retry.Delay = TimeSpan.FromMilliseconds(300);

            options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(10);
            options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(3);
        });
    })
    .Build();

var client = host.Services.GetRequiredService<ResilientBankClient>();
Console.WriteLine($"HttpClient skonfigurowany: {client.GetType().Name}");
Console.WriteLine("Pipeline: rate limiter, timeout, retry, circuit breaker, attempt timeout");
