namespace Exercise1_1.Logging;

public class LoggerPlikowy : ILogger
{
    private StreamWriter? _writer;
    private bool _disposed;

    public LoggerPlikowy(string sciezkaPliku)
    {
        _writer = new StreamWriter(sciezkaPliku, append: true);
        _writer.AutoFlush = true;
    }

    public void Loguj(PoziomLogu poziom, string wiadomosc)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        _writer!.WriteLine(
            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{poziom,-8}] {wiadomosc}");
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _writer?.Dispose();
            _writer = null;
        }

        _disposed = true;
    }

    ~LoggerPlikowy() => Dispose(disposing: false);
}
