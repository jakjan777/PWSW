using KanbanSystem.Domain.ValueObjects;
using MediatR;

namespace KanbanSystem.Application.Cards;

public sealed record CreateCardCommand(
    string Title,
    int PriorityLevel,
    Guid ColumnId) : IRequest<CreateCardResult>;

public sealed record CreateCardResult(Guid CardId, DateTime CreatedAt);
