using System.Xml.Linq;
using Microsoft.Extensions.Logging;

namespace EtlImport;

public class XmlEtlPipeline(ILogger<XmlEtlPipeline> logger) : EtlPipeline(logger)
{
    protected override async Task<IReadOnlyList<Dictionary<string, string>>> ExtractAsync(
        string source, CancellationToken ct)
    {
        await using var stream = File.OpenRead(source);
        var doc = await XDocument.LoadAsync(stream, LoadOptions.None, ct);
        var records = new List<Dictionary<string, string>>();

        foreach (var element in doc.Root?.Elements("record") ?? [])
        {
            var rec = element.Attributes()
                .ToDictionary(a => a.Name.LocalName, a => a.Value, StringComparer.OrdinalIgnoreCase);
            foreach (var child in element.Elements())
                rec[child.Name.LocalName] = child.Value;
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
}
