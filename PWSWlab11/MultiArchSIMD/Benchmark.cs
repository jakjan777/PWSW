using System.Diagnostics;

namespace MultiArchSIMD;

public static class Benchmark
{
    public static void Run(int size = 1_000_000, int iterations = 100)
    {
        var a = Enumerable.Range(0, size).Select(i => (float)i).ToArray();
        var b = Enumerable.Range(0, size).Select(i => (float)(i * 2)).ToArray();

        VectorOps.Add(a, b);

        var sw = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
            VectorOps.Add(a, b);
        sw.Stop();

        Console.WriteLine($"Rozmiar: {size:N0} elementow");
        Console.WriteLine($"Iteracje: {iterations}");
        Console.WriteLine($"Sciezka SIMD: {VectorOps.GetActivePath()}");
        Console.WriteLine($"Sredni czas: {sw.ElapsedMilliseconds / (double)iterations:F2} ms/iter");
        Console.WriteLine($"Calkowity czas: {sw.ElapsedMilliseconds} ms");
    }
}
