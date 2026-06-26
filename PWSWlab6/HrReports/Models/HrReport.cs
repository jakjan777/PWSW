using HrReports.Models;

namespace HrReports.Models;

public class ReportSection
{
    public string Type { get; init; } = "";
    public string Heading { get; init; } = "";
    public string Content { get; init; } = "";
    public ChartType? Chart { get; init; }
}

public class HrReport
{
    public string Title { get; init; } = "";
    public string Author { get; init; } = "";
    public ReportFormat Format { get; init; }
    public DateTime GeneratedAt { get; init; }
    public List<ReportSection> Sections { get; init; } = [];
    public Dictionary<string, string> Metadata { get; init; } = [];
}
