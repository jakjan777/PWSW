using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace EtlImport;

public class JsonEtlPipeline(ILogger<JsonEtlPipeline> logger) : EtlPipeline(logger)
{
    protected override async Task<IReadOnlyList<Dictionary<string, string>>> ExtractAsync(
        string source, CancellationToken ct)
    {
        await using var stream = File.OpenRead(source);
        var items = await JsonSerializer.DeserializeAsync<List<Dictionary<string, string>>>(stream, cancellationToken: ct);
        return items ?? [];
    }

    protected override ImportedRecord Transform(Dictionary<string, string> raw) =>
        new(
            raw.GetValueOrDefault("SourceId", raw.GetValueOrDefault("Id", Guid.NewGuid().ToString())),
            raw.GetValueOrDefault("Kategoria", "Nieznana"),
            raw.ToDictionary(kv => kv.Key, kv => (object)kv.Value),
            DateTime.UtcNow);

    protected override bool ValidateRecord(ImportedRecord record) =>
        !string.IsNullOrWhiteSpace(record.Id);
}
