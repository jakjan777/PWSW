namespace KanbanSystem.Domain.ValueObjects;

public sealed record CardPriority
{
    public static readonly CardPriority Low = new("Low", 1);
    public static readonly CardPriority Medium = new("Medium", 2);
    public static readonly CardPriority High = new("High", 3);

    public string Name { get; }
    public int Level { get; }

    private CardPriority(string name, int level) =>
        (Name, Level) = (name, level);

    public static CardPriority FromLevel(int level) => level switch
    {
        1 => Low,
        2 => Medium,
        3 => High,
        _ => throw new DomainException($"Nieprawidlowy poziom: {level}")
    };
}
