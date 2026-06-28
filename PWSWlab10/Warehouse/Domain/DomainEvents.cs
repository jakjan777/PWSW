using MediatR;

namespace Warehouse.Domain;

public interface IDomainEvent : INotification
{
    DateTime OccurredAt { get; }
}

public record ProductCreatedEvent(Guid Id, string Sku, string Name)
    : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record StockReceivedEvent(
    Guid ProductId, int Qty, int NewTotal, string ReceivedBy)
    : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record StockIssuedEvent(
    Guid ProductId, int Qty, int NewTotal,
    string IssuedBy, string Reason)
    : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record LowStockAlertEvent(
    Guid ProductId, string Sku, string Name,
    int CurrentQty, int ReorderLevel)
    : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
