using Challenge1;

Console.WriteLine("=== Wyzwanie 1: DockerServiceProxy ===\n");

var real = new FakeDockerService();
var proxy = new DockerServiceProxy(real, "secret-token");

Console.WriteLine("--- ListContainersAsync (1. wywolanie - z serwisu) ---");
var list1 = await proxy.ListContainersAsync();
Console.WriteLine(string.Join(", ", list1));
Console.WriteLine($"Wywolania real: {real.ListCallCount}");

Console.WriteLine("\n--- ListContainersAsync (2. wywolanie - z cache) ---");
var list2 = await proxy.ListContainersAsync();
Console.WriteLine(string.Join(", ", list2));
Console.WriteLine($"Wywolania real: {real.ListCallCount}");

Console.WriteLine("\n--- GetContainerLogsAsync (bez cache) ---");
Console.WriteLine(await proxy.GetContainerLogsAsync("web-1"));

Console.WriteLine("\n--- StartContainerAsync (czysci cache) ---");
await proxy.StartContainerAsync("web-1");
await proxy.ListContainersAsync();
Console.WriteLine($"Wywolania real po starcie: {real.ListCallCount}");

Console.WriteLine("\n--- Brak tokenu ---");
var proxyBezTokenu = new DockerServiceProxy(real, "");
try
{
    await proxyBezTokenu.ListContainersAsync();
}
catch (UnauthorizedAccessException ex)
{
    Console.WriteLine($"Oczekiwany blad: {ex.Message}");
}

Console.WriteLine("\n=== Koniec Wyzwania 1 ===");
