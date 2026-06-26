using System.Globalization;
using System.Text;
using DisposePatterns.Models;

namespace DisposePatterns.Disposal;

public class AsyncCsvFileReader : IAsyncDisposable
{
    private StreamReader? _reader;
    private bool _disposed;

    public AsyncCsvFileReader(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _reader = new StreamReader(filePath, Encoding.UTF8);
        _reader.ReadLine();
    }

    public async Task<SaleRecord?> ReadNextAsync(CancellationToken ct = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        string? line = await _reader!.ReadLineAsync(ct);
        if (line is null)
            return null;

        string[] parts = line.Split(';');
        return new SaleRecord(
            DateOnly.Parse(parts[0]),
            parts[1],
            parts[2],
            int.Parse(parts[3]),
            decimal.Parse(parts[4], CultureInfo.InvariantCulture));
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            if (_reader is IAsyncDisposable asyncDisposable)
                await asyncDisposable.DisposeAsync();
            else
                _reader?.Dispose();
            _reader = null;
            _disposed = true;
        }
    }
}
