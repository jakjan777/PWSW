namespace Warehouse.Domain;

public class AuditEntry
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string OperationId { get; init; } = "";
    public string UserId { get; init; } = "";
    public string Action { get; init; } = "";
    public string Details { get; init; } = "";
    public string? IpAddress { get; init; }
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
}
