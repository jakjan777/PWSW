using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace EtlImport;

public class CsvStreamEtl(ILogger<CsvStreamEtl> logger) : EtlPipeline(logger)
{
    protected override Task<IReadOnlyList<Dictionary<string, string>>> ExtractAsync(
        string source, CancellationToken ct)
    {
        var rekordy = new List<Dictionary<string, string>>();
        foreach (var wiersz in LeniwyCsvReader.CzytajWiersze(source))
        {
            ct.ThrowIfCancellationRequested();
            rekordy.Add(wiersz);
        }

        return Task.FromResult<IReadOnlyList<Dictionary<string, string>>>(rekordy);
    }

    protected override ImportedRecord Transform(Dictionary<string, string> raw) =>
        new(
            raw.GetValueOrDefault("Id", Guid.NewGuid().ToString()),
            raw.GetValueOrDefault("Kategoria", "Nieznana"),
            raw.ToDictionary(kv => kv.Key, kv => (object)kv.Value),
            DateTime.UtcNow);

    public static IEnumerable<string> PrzetwarzajStrumien(
        IEnumerable<Dictionary<string, string>> wiersze)
    {
        foreach (var wiersz in wiersze)
            yield return JsonSerializer.Serialize(wiersz);
    }

    public ImportResult RunStream(string source, CancellationToken ct = default)
    {
        ValidateSource(source);
        int rawCount = 0;
        int importedCount = 0;

        foreach (var raw in LeniwyCsvReader.CzytajWiersze(source))
        {
            ct.ThrowIfCancellationRequested();
            rawCount++;
            if (ValidateRecord(Transform(raw)))
                importedCount++;
        }

        return new ImportResult(rawCount, importedCount, rawCount - importedCount);
    }
}
