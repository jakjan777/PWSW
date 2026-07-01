using System.Text;

namespace PerformanceDemo;

public static class DataProcessorOptimized
{
    public static List<string> ProcessData(IEnumerable<string> items)
    {
        var result = new List<string>();
        var sb = new StringBuilder();

        foreach (var item in items)
        {
            sb.Clear();
            var upper = item.ToUpper();
            for (int i = 0; i < 100; i++)
                sb.Append(upper).Append(',');

            result.Add(sb.ToString());
        }

        result.Sort();
        return result;
    }
}
