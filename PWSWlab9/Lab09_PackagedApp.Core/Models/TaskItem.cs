namespace Lab09_PackagedApp.Core.Models;

public class TaskItem
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; init; } = "";
    public bool IsCompleted { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.Now;
}
