using Exercise2_1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

Console.WriteLine("=== Cwiczenie 2.1: Strukturalne logowanie w systemie bankowym ===\n");

var services = new ServiceCollection();
services.AddLogging(builder =>
{
    builder.AddSimpleConsole(options =>
    {
        options.IncludeScopes = true;
        options.SingleLine = true;
        options.TimestampFormat = "HH:mm:ss ";
    });
    builder.SetMinimumLevel(LogLevel.Debug);
});

services.AddSingleton<SerwisAutoryzacji>();
services.AddSingleton<SerwisTransakcji>();

var provider = services.BuildServiceProvider();
var serwis = provider.GetRequiredService<SerwisTransakcji>();

Console.WriteLine("--- Sesja 1: udane logowanie admin ---");
serwis.ObsluzSesje("admin", "192.168.1.100");

Console.WriteLine("\n--- Sesja 2: nieudane logowanie ---");
serwis.ObsluzSesje("hacker", "203.0.113.1");

Console.WriteLine("\n--- Transakcje (LoggerMessage source generator) ---");
serwis.WykonajPrzelew("Jan Kowalski", "Anna Nowak", 500m);
serwis.WykonajPrzelew("Firma ABC", "Dostawca XYZ", 25_000m);

Console.WriteLine("\n=== Koniec cwiczenia 2.1 ===");
