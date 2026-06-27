using Challenge2.Application;
using Challenge2.Domain;
using Challenge2.Infrastructure;

Console.WriteLine("=== Wyzwanie 2: AppVersion, VersionParser, BumpVersionUseCase ===\n");

Console.WriteLine("--- Parsowanie wersji (GeneratedRegex) ---");
foreach (var sample in new[] { "1.0.0.0", "2.3.4", "invalid" })
{
    try
    {
        var v = AppVersion.Parse(sample);
        Console.WriteLine($"  '{sample}' -> {v}");
    }
    catch (FormatException ex)
    {
        Console.WriteLine($"  '{sample}' -> blad: {ex.Message}");
    }
}

var manifestSource = Path.GetFullPath(
    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Lab09_PackagedApp", "Package.appxmanifest"));
var manifestCopy = Path.Combine(Path.GetTempPath(), "Lab09_Package.appxmanifest");

File.Copy(manifestSource, manifestCopy, overwrite: true);

Console.WriteLine($"\n--- Bump wersji w manifeście ---");
Console.WriteLine($"  Plik: {manifestCopy}");

var repo = new ManifestVersionRepository();
var useCase = new BumpVersionUseCase(repo);

var before = await repo.ReadAsync(manifestCopy);
Console.WriteLine($"  Przed: {before}");

var after = await useCase.ExecuteAsync(manifestCopy);
Console.WriteLine($"  Po bump: {after}");

var verify = await repo.ReadAsync(manifestCopy);
Console.WriteLine($"  Odczyt z pliku: {verify}");

File.Copy(manifestSource, manifestCopy, overwrite: true);
Console.WriteLine("\n  Przywrocono oryginalna wersje w kopii manifestu");

Console.WriteLine("\n=== Koniec wyzwania 2 ===");
