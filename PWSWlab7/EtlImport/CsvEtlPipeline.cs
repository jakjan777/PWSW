using Microsoft.Extensions.Logging;

namespace EtlImport;

public class CsvEtlPipeline(ILogger<CsvEtlPipeline> logger) : EtlPipeline(logger)
{
    protected override async Task<IReadOnlyList<Dictionary<string, string>>> ExtractAsync(
        string source, CancellationToken ct)
    {
        var records = new List<Dictionary<string, string>>();
        var lines = await File.ReadAllLinesAsync(source, ct);
        if (lines.Length == 0) return records;

        var headers = lines[0].Split(';');
        for (int i = 1; i < lines.Length; i++)
        {
            var values = lines[i].Split(';');
            var rec = new Dictionary<string, string>();
            for (int j = 0; j < headers.Length && j < values.Length; j++)
                rec[headers[j].Trim()] = values[j].Trim();
            records.Add(rec);
        }

        return records;
    }

    protected override ImportedRecord Transform(Dictionary<string, string> raw) =>
        new(
            raw.GetValueOrDefault("Id", Guid.NewGuid().ToString()),
            raw.GetValueOrDefault("Kategoria", "Nieznana"),
            raw.ToDictionary(kv => kv.Key, kv => (object)kv.Value),
            DateTime.UtcNow);

    protected override Task OnAfterLoadAsync(string source, CancellationToken ct)
    {
        var archivePath = Path.Combine(
            Path.GetDirectoryName(source)!,
            "archive",
            $"{DateTime.UtcNow:yyyyMMdd}_{Path.GetFileName(source)}");
        Directory.CreateDirectory(Path.GetDirectoryName(archivePath)!);
        if (File.Exists(source))
            File.Move(source, archivePath, overwrite: true);
        Logger.LogInformation("Plik CSV zarchiwizowany: {Path}", archivePath);
        return Task.CompletedTask;
    }
}
