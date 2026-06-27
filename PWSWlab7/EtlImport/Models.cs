namespace EtlImport;

public record ImportedRecord(
    string Id,
    string Kategoria,
    IReadOnlyDictionary<string, object> Pola,
    DateTime ImportedAt);

public record ImportResult(int RawCount, int ImportedCount, int SkippedCount);
