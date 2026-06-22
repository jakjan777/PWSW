namespace PWSWlab3.ViewModels;

public class NoteListViewModel
{
    public IReadOnlyList<NoteSummary> Notes { get; init; } = [];

    public string? SearchQuery { get; init; }

    public int TotalCount { get; init; }

    public int CurrentPage { get; init; } = 1;

    public string DecoratorStatus { get; init; } = string.Empty;
}

public class NoteSummary
{
    public Guid Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? Category { get; init; }

    public bool IsPinned { get; init; }

    public DateTime ModifiedAt { get; init; }

    public string AgeLabel =>
        (DateTime.Now - ModifiedAt).TotalDays switch
        {
            < 1 => "Dzisiaj",
            < 7 => "W tym tygodniu",
            _ => ModifiedAt.ToString("d")
        };
}
