using HrReports.Models;

namespace HrReports.Builders;

public class StepReportBuilder :
    IReportNeedsTitle,
    IReportNeedsAuthor,
    IReportCanAddSections,
    IReportCanBuild
{
    private string _title = "";
    private string _author = "";
    private ReportFormat _format = ReportFormat.Pdf;
    private readonly List<ReportSection> _sections = [];

    private StepReportBuilder() { }
    public static IReportNeedsTitle Create() => new StepReportBuilder();

    public IReportNeedsAuthor WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public IReportCanAddSections WithAuthor(string author)
    {
        _author = author;
        return this;
    }

    public IReportCanAddSections AddText(string heading, string content)
    {
        _sections.Add(new ReportSection
        {
            Type = "Text",
            Heading = heading,
            Content = content,
        });
        return this;
    }

    public IReportCanAddSections AddChart(string heading, ChartType chart)
    {
        _sections.Add(new ReportSection
        {
            Type = "Chart",
            Heading = heading,
            Chart = chart,
        });
        return this;
    }

    public IReportCanBuild InFormat(ReportFormat format)
    {
        _format = format;
        return this;
    }

    public HrReport Build() => new()
    {
        Title = _title,
        Author = _author,
        Format = _format,
        GeneratedAt = DateTime.Now,
        Sections = [.. _sections],
    };
}
