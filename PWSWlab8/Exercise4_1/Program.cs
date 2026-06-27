using Exercise4_1.Creators;
using Exercise4_1.Factory;

Console.WriteLine("=== Cwiczenie 4.1: Fabryka dokumentow prawnych ===\n");

Console.WriteLine("--- Factory Method: kreatory dokumentow ---\n");

DocumentCreator contractCreator = new ContractCreator(
    "Firma XYZ", 150_000m, new DateTime(2027, 12, 31));

var contract = contractCreator.PrepareDocument("Kancelaria ABC");
Console.WriteLine(contract.GenerateContent());

Console.WriteLine();

DocumentCreator demandCreator = new PaymentDemandCreator("Jan Nowak", 5000m, 14);
var demand = demandCreator.PrepareDocument("Kancelaria ABC");
Console.WriteLine(demand.GenerateContent());

Console.WriteLine("\n--- Fabryka parametryzowana: konfiguracja kontenerow ---\n");

var factory = new ContainerConfigFactory()
    .Register("dev", app => new ContainerConfigFactory.ContainerConfig(
        $"{app}:dev", 512, 1,
        new() { ["ASPNETCORE_ENVIRONMENT"] = "Development" }))
    .Register("prod", app => new ContainerConfigFactory.ContainerConfig(
        $"{app}:latest", 2048, 4,
        new() { ["ASPNETCORE_ENVIRONMENT"] = "Production" }));

var devConfig = factory.Create("dev", "lab08-app");
var prodConfig = factory.Create("prod", "lab08-app");

Console.WriteLine($"Dev:  {devConfig.ImageName}, RAM: {devConfig.MemoryMB} MB, CPU: {devConfig.CpuShares}");
Console.WriteLine($"Prod: {prodConfig.ImageName}, RAM: {prodConfig.MemoryMB} MB, CPU: {prodConfig.CpuShares}");

Console.WriteLine("\n=== Koniec cwiczenia 4.1 ===");
