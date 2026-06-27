using KanbanSystem.Domain.Entities;

namespace KanbanSystem.Application.Interfaces;

public interface ICardRepository
{
    Task<Card?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Card card, CancellationToken ct = default);
}

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
