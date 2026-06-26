using DisposePatterns.Native;

namespace DisposePatterns.Disposal;

public class SalesReportWriter : IDisposable
{
    private StreamWriter? _writer;
    private IntPtr _nativeLogHandle;
    private bool _disposed;

    public SalesReportWriter(string outputPath)
    {
        _writer = new StreamWriter(outputPath, append: false);
        _nativeLogHandle = NativeLogger.Open("sales.log");
    }

    public void WriteLine(string line) =>
        _writer!.WriteLine(line);

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
                _writer?.Dispose();

            if (_nativeLogHandle != IntPtr.Zero)
            {
                NativeLogger.Close(_nativeLogHandle);
                _nativeLogHandle = IntPtr.Zero;
            }

            _disposed = true;
        }
    }

    ~SalesReportWriter() => Dispose(disposing: false);

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
