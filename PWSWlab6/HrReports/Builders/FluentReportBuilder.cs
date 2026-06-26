using HrReports.Models;

namespace HrReports.Builders;

public class FluentReportBuilder
{
    private string _title = "";
    private string _author = "";
    private ReportFormat _format = ReportFormat.Pdf;
    private readonly List<ReportSection> _sections = [];
    private readonly Dictionary<string, string> _metadata = [];

    public FluentReportBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public FluentReportBuilder WithAuthor(string author)
    {
        _author = author;
        return this;
    }

    public FluentReportBuilder InFormat(ReportFormat format)
    {
        _format = format;
        return this;
    }

    public FluentReportBuilder AddText(string heading, string content)
    {
        _sections.Add(new ReportSection
        {
            Type = "Text",
            Heading = heading,
            Content = content,
        });
        return this;
    }

    public FluentReportBuilder AddChart(string heading, ChartType chart)
    {
        _sections.Add(new ReportSection
        {
            Type = "Chart",
            Heading = heading,
            Chart = chart,
        });
        return this;
    }

    public FluentReportBuilder WithMetadata(string key, string value)
    {
        _metadata[key] = value;
        return this;
    }

    public HrReport Build() => new()
    {
        Title = _title,
        Author = _author,
        Format = _format,
        GeneratedAt = DateTime.Now,
        Sections = [.. _sections],
        Metadata = new(_metadata),
    };
}
