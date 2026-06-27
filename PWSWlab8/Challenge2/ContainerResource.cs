namespace Challenge2;

public enum ContainerType
{
    Dev,
    Test,
    Production
}

public enum ContainerHealth
{
    Healthy,
    Degraded,
    Unhealthy
}

public interface IContainerResource : IDisposable
{
    string ContainerId { get; }
    ContainerType Type { get; }
    Task InitializeAsync();
    Task<ContainerHealth> CheckHealthAsync();
}

public abstract class ContainerResourceBase : IContainerResource
{
    public string ContainerId { get; }
    protected bool _disposed;

    protected ContainerResourceBase(string containerId)
    {
        ContainerId = containerId;
    }

    public abstract ContainerType Type { get; }
    public abstract Task InitializeAsync();

    public virtual async Task<ContainerHealth> CheckHealthAsync()
    {
        await Task.Delay(10);
        return ContainerHealth.Healthy;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
            Console.WriteLine($"Zwalniam zasob kontenera {ContainerId}");

        _disposed = true;
    }

    ~ContainerResourceBase() => Dispose(disposing: false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

public class DevContainer(string id) : ContainerResourceBase(id)
{
    public override ContainerType Type => ContainerType.Dev;

    public override async Task InitializeAsync()
    {
        await Task.Delay(20);
        Console.WriteLine($"[Dev] Inicjalizacja {ContainerId}");
    }
}

public class TestContainer(string id) : ContainerResourceBase(id)
{
    public override ContainerType Type => ContainerType.Test;

    public override async Task InitializeAsync()
    {
        await Task.Delay(30);
        Console.WriteLine($"[Test] Inicjalizacja {ContainerId}");
    }
}

public class ProductionContainer(string id) : ContainerResourceBase(id)
{
    public override ContainerType Type => ContainerType.Production;

    public override async Task InitializeAsync()
    {
        await Task.Delay(50);
        Console.WriteLine($"[Prod] Inicjalizacja {ContainerId}");
    }

    public override async Task<ContainerHealth> CheckHealthAsync()
    {
        await Task.Delay(20);
        return Random.Shared.Next(2) == 0
            ? ContainerHealth.Healthy
            : ContainerHealth.Degraded;
    }
}

public static class ContainerResourceFactory
{
    public static IContainerResource Create(ContainerType type, string id) =>
        type switch
        {
            ContainerType.Dev => new DevContainer(id),
            ContainerType.Test => new TestContainer(id),
            ContainerType.Production => new ProductionContainer(id),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
}
