using DisposePatterns.Disposal;
using DisposePatterns.Models;

var dataPath = Path.Combine(AppContext.BaseDirectory, "Data", "sprzedaz.csv");
var reportPath = Path.Combine(AppContext.BaseDirectory, "raport.txt");

Console.WriteLine("=== Zarzadzanie zasobami ===\n");

Console.WriteLine("CsvFileReader z using:");
PrzetworzPlik(dataPath);

Console.WriteLine("\nSalesReportWriter z pelnym wzorcem Dispose:");
using (var writer = new SalesReportWriter(reportPath))
{
    writer.WriteLine("Raport sprzedazy");
    writer.WriteLine($"Wygenerowano: {DateTime.Now:yyyy-MM-dd HH:mm}");
    writer.WriteLine("Oddzial;Produkt;Kwota");
    writer.WriteLine("Bydgoszcz;Laptop;9999.00");
}
Console.WriteLine($"  Raport zapisany: {reportPath}");
Console.WriteLine($"  Uchwyt natywny zamkniety po Dispose");

Console.WriteLine("\nAsyncCsvFileReader z await using:");
await PrzetworzPlikAsync(dataPath);

static void PrzetworzPlik(string sciezka)
{
    using var reader = new CsvFileReader(sciezka);
    while (reader.ReadNext() is SaleRecord r)
        Console.WriteLine($"  {r.Branch}: {r.Product} -> {r.Total:C}");
}

static async Task PrzetworzPlikAsync(string sciezka)
{
    await using var reader = new AsyncCsvFileReader(sciezka);
    while (await reader.ReadNextAsync() is SaleRecord r)
        Console.WriteLine($"  {r.Branch}: {r.Product} -> {r.Total:C}");
}
