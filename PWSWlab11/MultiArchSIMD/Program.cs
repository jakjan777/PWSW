using MultiArchSIMD;

PrismDetector.PrintInfo();

Console.WriteLine();
Console.WriteLine("=== Benchmark SIMD ===");
Benchmark.Run();

var a = new float[] { 1f, 2f, 3f, 4f, 5f };
var b = new float[] { 10f, 20f, 30f, 40f, 50f };
var result = VectorOps.Add(a, b);

Console.WriteLine();
Console.WriteLine("Weryfikacja poprawnosci:");
Console.WriteLine($"  [{string.Join(", ", result)}]");
