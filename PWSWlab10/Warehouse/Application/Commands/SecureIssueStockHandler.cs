using System.ComponentModel.DataAnnotations;
using MediatR;
using Warehouse.Domain;
using Warehouse.Infrastructure;

namespace Warehouse.Application.Commands;

public record SecureIssueStockCommand(
    Guid ProductId,
    [property: Range(1, int.MaxValue, ErrorMessage = "Ilosc musi byc >= 1")]
    int Qty,
    [property: Required(ErrorMessage = "IssuedBy jest wymagane")]
    string IssuedBy,
    [property: Required(ErrorMessage = "Powod jest wymagany")]
    [property: StringLength(500, MinimumLength = 1)]
    string Reason,
    string? IpAddress) : IRequest;

public class SecureIssueStockHandler(
    WriteDbContext db, IMediator mediator)
    : IRequestHandler<SecureIssueStockCommand>
{
    public async Task Handle(SecureIssueStockCommand cmd, CancellationToken ct)
    {
        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(
                cmd, new ValidationContext(cmd), results, true))
            throw new ValidationException(
                string.Join("; ", results.Select(r => r.ErrorMessage)));

        var product = await db.Products
            .FindAsync([cmd.ProductId], ct)
            ?? throw new KeyNotFoundException(
                $"Produkt {cmd.ProductId} nie istnieje.");

        product.IssueStock(cmd.Qty, cmd.IssuedBy, cmd.Reason);

        var operationId = Guid.NewGuid().ToString("N");
        db.AuditLog.Add(new AuditEntry
        {
            OperationId = operationId,
            UserId = cmd.IssuedBy,
            Action = "IssueStock",
            Details = $"ProductId={cmd.ProductId} Qty={cmd.Qty} Reason={cmd.Reason}",
            IpAddress = cmd.IpAddress
        });

        await db.SaveChangesAsync(ct);

        foreach (var ev in product.DomainEvents)
            await mediator.Publish(ev, ct);
        product.ClearEvents();
    }
}
