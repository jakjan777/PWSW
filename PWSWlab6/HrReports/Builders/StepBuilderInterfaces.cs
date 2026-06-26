using HrReports.Models;

namespace HrReports.Builders;

public interface IReportNeedsTitle
{
    IReportNeedsAuthor WithTitle(string title);
}

public interface IReportNeedsAuthor
{
    IReportCanAddSections WithAuthor(string author);
}

public interface IReportCanAddSections
{
    IReportCanAddSections AddText(string heading, string content);
    IReportCanAddSections AddChart(string heading, ChartType chart);
    IReportCanBuild InFormat(ReportFormat format);
}

public interface IReportCanBuild
{
    HrReport Build();
}
