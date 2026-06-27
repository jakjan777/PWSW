using EtlImport;
using Microsoft.Extensions.Logging;

var dataDir = Path.Combine(AppContext.BaseDirectory, "SampleData");
Directory.CreateDirectory(dataDir);

PrepareSampleFiles(dataDir);

using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

Console.WriteLine("--- Import CSV ---");
var csvPath = Path.Combine(dataDir, "dane.csv");
var csvResult = await new CsvEtlPipeline(loggerFactory.CreateLogger<CsvEtlPipeline>()).RunAsync(csvPath);
Console.WriteLine($"Wynik CSV: surowe={csvResult.RawCount}, zaimportowane={csvResult.ImportedCount}, pominiete={csvResult.SkippedCount}");
Console.WriteLine($"Plik zrodlowy istnieje: {File.Exists(csvPath)}");
Console.WriteLine($"Archiwum istnieje: {Directory.Exists(Path.Combine(dataDir, "archive"))}");

Console.WriteLine();
Console.WriteLine("--- Import JSON ---");
var jsonResult = await new JsonEtlPipeline(loggerFactory.CreateLogger<JsonEtlPipeline>())
    .RunAsync(Path.Combine(dataDir, "dane.json"));
Console.WriteLine($"Wynik JSON: surowe={jsonResult.RawCount}, zaimportowane={jsonResult.ImportedCount}, pominiete={jsonResult.SkippedCount}");

Console.WriteLine();
Console.WriteLine("--- Import XML ---");
var xmlResult = await new XmlEtlPipeline(loggerFactory.CreateLogger<XmlEtlPipeline>())
    .RunAsync(Path.Combine(dataDir, "dane.xml"));
Console.WriteLine($"Wynik XML: surowe={xmlResult.RawCount}, zaimportowane={xmlResult.ImportedCount}, pominiete={xmlResult.SkippedCount}");

Console.WriteLine();
Console.WriteLine("--- Wyzwanie 2: Leniwy CSV (10 000 wierszy) ---");
var duzyPlik = Path.Combine(dataDir, "dostawcy_10000.csv");
GenerujDuzyCsv(duzyPlik, 10_000);

GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();
long pamiecStart = GC.GetTotalMemory(true);

int licznik = 0;
long pamiecMax = pamiecStart;
foreach (var _ in CsvStreamEtl.PrzetwarzajStrumien(LeniwyCsvReader.CzytajWiersze(duzyPlik)))
{
    licznik++;
    if (licznik % 2500 == 0)
    {
        long aktualna = GC.GetTotalMemory(false);
        pamiecMax = Math.Max(pamiecMax, aktualna);
        Console.WriteLine($"  {licznik} wierszy, pamiec: {aktualna / 1024 / 1024} MB");
    }
}

GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();
long pamiecKoniec = GC.GetTotalMemory(true);

Console.WriteLine($"Przetworzono: {licznik} wierszy");
Console.WriteLine($"Pamiec start: {pamiecStart / 1024 / 1024} MB, max w trakcie: {pamiecMax / 1024 / 1024} MB, koniec: {pamiecKoniec / 1024 / 1024} MB");
Console.WriteLine($"Przyrost pamieci: {(pamiecKoniec - pamiecStart) / 1024 / 1024} MB (staly, bez wczytywania calego pliku)");

var streamResult = new CsvStreamEtl(loggerFactory.CreateLogger<CsvStreamEtl>()).RunStream(duzyPlik);
Console.WriteLine($"RunStream ETL: surowe={streamResult.RawCount}, zaimportowane={streamResult.ImportedCount}");

static void GenerujDuzyCsv(string path, int wiersze)
{
    using var writer = new StreamWriter(path);
    writer.WriteLine("Id;Kategoria;Produkt");
    for (int i = 1; i <= wiersze; i++)
        writer.WriteLine($"{i};Kategoria-{i % 20};Produkt-{i:D5}");
}

static void PrepareSampleFiles(string dir)
{
    File.WriteAllText(Path.Combine(dir, "dane.csv"),
        """
        Id;Kategoria;Produkt
        1;Elektronika;Laptop
        2;Elektronika;Monitor
        """);

    File.WriteAllText(Path.Combine(dir, "dane.json"),
        """
        [
          { "SourceId": "J-100", "Kategoria": "AGD", "Produkt": "Ekspres" },
          { "SourceId": "", "Kategoria": "AGD", "Produkt": "Brak ID" }
        ]
        """);

    File.WriteAllText(Path.Combine(dir, "dane.xml"),
        """
        <records>
          <record Id="X-1" Kategoria="Meble">
            <Produkt>Biurko</Produkt>
          </record>
          <record Id="X-2" Kategoria="Meble">
            <Produkt>Krzeslo</Produkt>
          </record>
        </records>
        """);
}
