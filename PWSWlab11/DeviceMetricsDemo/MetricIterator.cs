using System.Collections;
using System.Reflection;

namespace DeviceMetricsDemo;

[AttributeUsage(AttributeTargets.Property)]
public class UnitAttribute(string unit) : Attribute
{
    public string Unit { get; } = unit;
}

[AttributeUsage(AttributeTargets.Property)]
public class AlertAttribute(double threshold) : Attribute
{
    public double Threshold { get; } = threshold;
}

[AttributeUsage(AttributeTargets.Property)]
public class MetricNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}

public class DeviceMetrics
{
    [MetricName("Temperatura CPU")]
    [Unit("deg C")]
    [Alert(90.0)]
    public double CpuTemperature { get; set; }

    [MetricName("Obciazenie CPU")]
    [Unit("%")]
    [Alert(95.0)]
    public double CpuLoad { get; set; }

    [MetricName("Uzycie pamieci")]
    [Unit("%")]
    [Alert(85.0)]
    public double MemoryUsage { get; set; }
}

public record MetricResult(string Name, double Value, string Unit, bool IsAlert);

public class MetricIterator(DeviceMetrics metrics) : IEnumerable<MetricResult>
{
    public IEnumerator<MetricResult> GetEnumerator()
    {
        foreach (PropertyInfo prop in typeof(DeviceMetrics).GetProperties())
        {
            var metricName = prop.GetCustomAttribute<MetricNameAttribute>();
            if (metricName is null)
                continue;

            var value = Convert.ToDouble(prop.GetValue(metrics)!);
            var unit = prop.GetCustomAttribute<UnitAttribute>()?.Unit ?? "";
            var alert = prop.GetCustomAttribute<AlertAttribute>();
            var isAlert = alert is not null && value > alert.Threshold;

            yield return new MetricResult(metricName.Name, value, unit, isAlert);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
