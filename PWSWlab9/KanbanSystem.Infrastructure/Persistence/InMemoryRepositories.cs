using KanbanSystem.Application.Interfaces;
using KanbanSystem.Domain.Entities;

namespace KanbanSystem.Infrastructure.Persistence;

public class InMemoryCardRepository : ICardRepository
{
    private readonly List<Card> _cards = [];

    public Task<Card?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => Task.FromResult(_cards.FirstOrDefault(c => c.Id == id));

    public Task AddAsync(Card card, CancellationToken ct = default)
    {
        _cards.Add(card);
        return Task.CompletedTask;
    }

    public IReadOnlyList<Card> GetAll() => _cards;
}

public class InMemoryUnitOfWork : IUnitOfWork
{
    public int SaveCount { get; private set; }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        SaveCount++;
        return Task.FromResult(1);
    }
}
