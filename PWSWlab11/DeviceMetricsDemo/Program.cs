using DeviceMetricsDemo;

var metrics = new DeviceMetrics
{
    CpuTemperature = 92.5,
    CpuLoad = 78.0,
    MemoryUsage = 91.2
};

var iterator = new MetricIterator(metrics);
foreach (var metric in iterator)
{
    string alert = metric.IsAlert ? " [!]" : "";
    Console.WriteLine($"{metric.Name}: {metric.Value:F1} {metric.Unit}{alert}");
}
