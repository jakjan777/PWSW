namespace SecurityScanner;

public record SubsystemReport(
    string Name,
    SubsystemStatus Status,
    string Details,
    TimeSpan CheckTime);
