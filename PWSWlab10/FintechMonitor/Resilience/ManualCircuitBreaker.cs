namespace FintechMonitor.Resilience;

public enum CircuitState { Closed, Open, HalfOpen }

public sealed class ManualCircuitBreaker(
    int failureThreshold = 3,
    TimeSpan? openDuration = null)
{
    private readonly TimeSpan _openFor =
        openDuration ?? TimeSpan.FromSeconds(30);
    private readonly Lock _lock = new();

    private CircuitState _state = CircuitState.Closed;
    private int _failures;
    private DateTime _openedAt;

    public CircuitState State
    {
        get
        {
            lock (_lock)
            {
                if (_state == CircuitState.Open &&
                    DateTime.UtcNow - _openedAt >= _openFor)
                    _state = CircuitState.HalfOpen;
                return _state;
            }
        }
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        lock (_lock)
            if (State == CircuitState.Open)
                throw new CircuitBrokenException(
                    "Obwod otwarty --- zadanie odrzucone.");

        try
        {
            var result = await action();
            lock (_lock)
            {
                _failures = 0;
                _state = CircuitState.Closed;
            }
            return result;
        }
        catch
        {
            lock (_lock)
            {
                if (++_failures >= failureThreshold)
                {
                    _state = CircuitState.Open;
                    _openedAt = DateTime.UtcNow;
                }
            }
            throw;
        }
    }
}

public class CircuitBrokenException(string msg) : Exception(msg);
