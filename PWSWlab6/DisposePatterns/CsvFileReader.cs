using System.Globalization;
using System.Text;
using DisposePatterns.Models;

namespace DisposePatterns.Disposal;

public class CsvFileReader : IDisposable
{
    private StreamReader? _reader;
    private bool _disposed;

    public CsvFileReader(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _reader = new StreamReader(filePath, Encoding.UTF8);
        _reader.ReadLine();
    }

    public SaleRecord? ReadNext()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        string? line = _reader!.ReadLine();
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

    public void Dispose()
    {
        if (!_disposed)
        {
            _reader?.Dispose();
            _reader = null;
            _disposed = true;
        }
    }
}
