using System.Diagnostics;
using PerformanceDemo;

var data = Enumerable.Range(0, 1000).Select(i => $"item-{i}");

var sw = Stopwatch.StartNew();
DataProcessor.ProcessData(data);
sw.Stop();
Console.WriteLine($"Czas przed optymalizacja: {sw.ElapsedMilliseconds} ms");

var sw2 = Stopwatch.StartNew();
DataProcessorOptimized.ProcessData(data);
sw2.Stop();
Console.WriteLine($"Czas po optymalizacji: {sw2.ElapsedMilliseconds} ms");

if (sw2.ElapsedMilliseconds > 0)
    Console.WriteLine($"Przyspieszenie: {(double)sw.ElapsedMilliseconds / sw2.ElapsedMilliseconds:F1}x");
