using System.Text.RegularExpressions;
using Challenge2.Application;
using Challenge2.Domain;

namespace Challenge2.Infrastructure;

public partial class ManifestVersionRepository : IVersionRepository
{
    [GeneratedRegex(
        @"<Identity\b[\s\S]*?Version\s*=\s*""(?<version>[^""]+)""",
        RegexOptions.Compiled | RegexOptions.IgnoreCase)]
    private static partial Regex IdentityVersionRegex();

    public async Task<AppVersion> ReadAsync(string path, CancellationToken ct = default)
    {
        using var reader = new StreamReader(path);
        var content = await reader.ReadToEndAsync(ct);

        var match = IdentityVersionRegex().Match(content);
        if (!match.Success)
            throw new InvalidOperationException("Nie znaleziono atrybutu Version w elemencie Identity");

        return AppVersion.Parse(match.Groups["version"].Value);
    }

    public async Task WriteAsync(string path, AppVersion version, CancellationToken ct = default)
    {
        var content = await File.ReadAllTextAsync(path, ct);
        var match = IdentityVersionRegex().Match(content);
        if (!match.Success)
            throw new InvalidOperationException("Nie znaleziono atrybutu Version w elemencie Identity");

        var oldVersion = match.Groups["version"].Value;
        var updated = content.Replace(
            $"Version=\"{oldVersion}\"",
            $"Version=\"{version}\"",
            StringComparison.OrdinalIgnoreCase);

        await using var writer = new StreamWriter(path);
        await writer.WriteAsync(updated.AsMemory(), ct);
    }
}
