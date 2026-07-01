using FormularzFramework;

Console.WriteLine("=== Metadane Pracownik ===");
foreach (var meta in MetadataReader.CzytajMetadane<Pracownik>())
{
    Console.WriteLine($"  [{meta.Order}] {meta.DisplayName}" +
        $" | wymagane: {meta.IsRequired}" +
        $" | email: {meta.IsEmail}" +
        (meta.Range is not null ? $" | zakres: {meta.Range.Min}-{meta.Range.Max}" : ""));
}

var blednyPracownik = new Pracownik
{
    Imie = "",
    Nazwisko = "A".PadRight(60, 'x'),
    Email = "brak-at",
    Wiek = 15,
    Pensja = -100m
};

Console.WriteLine();
Console.WriteLine("=== Walidacja blednych danych ===");
var bledy = AtrybutValidator.Waliduj(blednyPracownik);
foreach (var blad in bledy)
    Console.WriteLine($"  - {blad}");
