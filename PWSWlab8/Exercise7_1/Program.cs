using System.Runtime.InteropServices;

Console.WriteLine("=== Cwiczenie 7.1: Kontener Windows ===\n");
Console.WriteLine("=== Informacje o platformie ===");
Console.WriteLine($"OS: {RuntimeInformation.OSDescription}");
Console.WriteLine($"Architektura: {RuntimeInformation.ProcessArchitecture}");
Console.WriteLine($"Framework: {RuntimeInformation.FrameworkDescription}");
Console.WriteLine($"WSL: {IsRunningInWSL()}");
Console.WriteLine($"Separator sciezek: '{Path.DirectorySeparatorChar}'");

static bool IsRunningInWSL()
{
    if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        return false;

    try
    {
        var version = File.ReadAllText("/proc/version");
        return version.Contains("microsoft", StringComparison.OrdinalIgnoreCase);
    }
    catch
    {
        return false;
    }
}
