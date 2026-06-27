using Exercise1_1;
using Exercise1_1.Logging;
using Exercise1_1.Rezerwacja;

Console.WriteLine("=== Cwiczenie 1.1: Logger z IDisposable i hierarchia wyjatkow ===\n");

var appLogPath = Path.Combine(AppContext.BaseDirectory, "app.log");
var errorsLogPath = Path.Combine(AppContext.BaseDirectory, "errors.log");

// using var -- Dispose() na koncu zakresu
using var logger = new LoggerKompozytowy(
    new LoggerPlikowy(appLogPath),
    new LoggerPlikowy(errorsLogPath));

logger.Loguj(PoziomLogu.Info, "System uruchomiony");
logger.Loguj(PoziomLogu.Warning, "Test ostrzezenia");
logger.Loguj(PoziomLogu.Error, "Test bledu");

Console.WriteLine("\n--- Obsluga wyjatkow rezerwacji (catch when) ---\n");

ObslugaRezerwacji.ObsluzRezerwacje(
    () => throw new OverbookingException("LO-505", dostepne: 0, wymagane: 1));

ObslugaRezerwacji.ObsluzRezerwacje(
    () => throw new OverbookingException("LO-101", dostepne: 2, wymagane: 5));

ObslugaRezerwacji.ObsluzRezerwacje(
    () => throw new PlatnoscException(299.99m, "TIMEOUT_SIECI", "LO-200"));

ObslugaRezerwacji.ObsluzRezerwacje(
    () => throw new PlatnoscException(150m, "KARTA_ODRZUCONA", "LO-300"));

Console.WriteLine("\n--- Demonstracja using (blok) vs using var ---\n");

using (var blokLogger = new LoggerPlikowy(Path.Combine(AppContext.BaseDirectory, "blok.log")))
{
    blokLogger.Loguj(PoziomLogu.Debug, "Log z bloku using (...) { }");
}

Console.WriteLine("Blok using zakonczony -- Dispose() wywolany automatycznie.");

Console.WriteLine($"\nPliki logow zapisane w: {AppContext.BaseDirectory}");
Console.WriteLine("  - app.log");
Console.WriteLine("  - errors.log");
Console.WriteLine("  - blok.log");

Console.WriteLine("\n=== Koniec cwiczenia 1.1 ===");
