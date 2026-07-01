namespace PerformanceDemo;

public static class DataProcessor
{
    public static List<string> ProcessData(IEnumerable<string> items)
    {
        var result = new List<string>();
        foreach (var item in items)
        {
            var processed = "";
            for (int i = 0; i < 100; i++)
                processed += item.ToUpper() + ",";

            result.Add(processed);
            var sorted = result.OrderBy(x => x).ToList();
        }
        return result;
    }
}
