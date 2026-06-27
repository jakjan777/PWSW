using Challenge2;

Console.WriteLine("=== Wyzwanie 2: Zasoby kontenerowe + Factory Method ===\n");

async Task RunScenario(ContainerType type, string id)
{
    Console.WriteLine($"--- {type} / {id} ---");
    using var resource = ContainerResourceFactory.Create(type, id);
    await resource.InitializeAsync();
    var health = await resource.CheckHealthAsync();
    Console.WriteLine($"Stan zdrowia: {health}");
    Console.WriteLine("Koniec using -- Dispose() wywolany automatycznie\n");
}

await RunScenario(ContainerType.Dev, "dev-local-01");
await RunScenario(ContainerType.Test, "test-ci-02");
await RunScenario(ContainerType.Production, "prod-main-03");

Console.WriteLine("--- Blad inicjalizacji z using ---");
try
{
    using var broken = ContainerResourceFactory.Create(ContainerType.Dev, "dev-fail");
    throw new InvalidOperationException("Symulowany blad podczas pracy");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Przechwycono: {ex.Message}");
    Console.WriteLine("Dispose() i tak zostal wywolany po wyjsciu z using\n");
}

Console.WriteLine("=== Koniec Wyzwania 2 ===");
