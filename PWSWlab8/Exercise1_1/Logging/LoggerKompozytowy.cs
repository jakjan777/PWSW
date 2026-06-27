namespace Exercise1_1.Logging;

public class LoggerKompozytowy : ILogger
{
    private readonly List<ILogger> _loggery;
    private bool _disposed;

    public LoggerKompozytowy(params ILogger[] loggery) =>
        _loggery = [.. loggery];

    public void Loguj(PoziomLogu poziom, string wiadomosc)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        foreach (var logger in _loggery)
            logger.Loguj(poziom, wiadomosc);
    }

    public void Dispose()
    {
        if (_disposed) return;

        List<Exception> bledy = [];
        foreach (var logger in _loggery)
        {
            try { logger.Dispose(); }
            catch (Exception ex) { bledy.Add(ex); }
        }

        _loggery.Clear();
        _disposed = true;

        if (bledy.Count > 0)
            throw new AggregateException("Bledy zwalniania", bledy);
    }
}
