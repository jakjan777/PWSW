Console.WriteLine("=== Cwiczenie 5.2: Strumieniowy odczyt AppxBlockMap.xml ===\n");

var blockMapPath = Path.Combine(
    @"C:\Users\jakja\source\repos\PWSWlab9\Lab09_PackagedApp",
    "msix-contents",
    "AppxBlockMap.xml");

if (!File.Exists(blockMapPath))
{
    Console.WriteLine("Brak pliku AppxBlockMap.xml. Uruchom najpierw inspect-msix.ps1");
    return;
}

await using var stream = new FileStream(
    blockMapPath, FileMode.Open, FileAccess.Read);
using var reader = new StreamReader(stream);

int blockCount = 0;
string? line;
while ((line = await reader.ReadLineAsync()) is not null)
{
    int idx = 0;
    while ((idx = line.IndexOf("<Block ", idx, StringComparison.Ordinal)) >= 0)
    {
        blockCount++;
        idx += 7;
    }
}

Console.WriteLine($"Sciezka: {blockMapPath}");
Console.WriteLine($"Liczba blokow w AppxBlockMap: {blockCount}");

Console.WriteLine("\n=== Koniec cwiczenia 5.2 (odczyt C#) ===");
