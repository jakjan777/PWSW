using KanbanSystem.Application.Interfaces;
using KanbanSystem.Domain.Entities;
using KanbanSystem.Domain.ValueObjects;
using MediatR;

namespace KanbanSystem.Application.Cards;

public sealed class CreateCardHandler(
    ICardRepository repo,
    IUnitOfWork uow) : IRequestHandler<CreateCardCommand, CreateCardResult>
{
    public async Task<CreateCardResult> Handle(
        CreateCardCommand req, CancellationToken ct)
    {
        var priority = CardPriority.FromLevel(req.PriorityLevel);
        var card = Card.Create(req.Title, priority, req.ColumnId);
        await repo.AddAsync(card, ct);
        await uow.SaveChangesAsync(ct);
        return new CreateCardResult(card.Id, card.CreatedAt);
    }
}
