using Microsoft.Extensions.Logging;

namespace EtlImport;

public abstract class EtlPipeline(ILogger logger)
{
    protected readonly ILogger Logger = logger;

    public async Task<ImportResult> RunAsync(string source, CancellationToken ct = default)
    {
        ValidateSource(source);
        await OnBeforeExtractAsync(source, ct);
        var rawRecords = await ExtractAsync(source, ct);
        var transformed = new List<ImportedRecord>();
        foreach (var raw in rawRecords)
        {
            var record = Transform(raw);
            if (ValidateRecord(record))
                transformed.Add(record);
            else
                Logger.LogWarning("Pominieto rekord: {Id}", record.Id);
        }

        await LoadAsync(transformed, ct);
        await OnAfterLoadAsync(source, ct);
        return new ImportResult(rawRecords.Count, transformed.Count, rawRecords.Count - transformed.Count);
    }

    protected abstract Task<IReadOnlyList<Dictionary<string, string>>> ExtractAsync(
        string source, CancellationToken ct);

    protected abstract ImportedRecord Transform(Dictionary<string, string> raw);

    protected void ValidateSource(string source)
    {
        if (!File.Exists(source))
            throw new FileNotFoundException($"Plik zrodlowy nie istnieje: {source}", source);
    }

    protected virtual Task LoadAsync(IReadOnlyList<ImportedRecord> records, CancellationToken ct)
    {
        foreach (var record in records)
            Logger.LogInformation("Zaladowano rekord {Id} ({Kategoria})", record.Id, record.Kategoria);
        return Task.CompletedTask;
    }

    protected virtual Task OnBeforeExtractAsync(string source, CancellationToken ct) =>
        Task.CompletedTask;

    protected virtual Task OnAfterLoadAsync(string source, CancellationToken ct) =>
        Task.CompletedTask;

    protected virtual bool ValidateRecord(ImportedRecord record) => true;
}
