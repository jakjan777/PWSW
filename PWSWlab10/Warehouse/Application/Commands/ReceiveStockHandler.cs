using System.Data;
using MediatR;
using Warehouse.Domain;
using Warehouse.Infrastructure;

namespace Warehouse.Application.Commands;

public record ReceiveStockCommand(
    Guid ProductId, int Qty, string ReceivedBy) : IRequest;

public class ReceiveStockHandler(
    WriteDbContext db, IMediator mediator)
    : IRequestHandler<ReceiveStockCommand>
{
    public async Task Handle(ReceiveStockCommand cmd, CancellationToken ct)
    {
        var product = await db.Products
            .FindAsync([cmd.ProductId], ct)
            ?? throw new KeyNotFoundException(
                $"Produkt {cmd.ProductId} nie istnieje.");

        product.ReceiveStock(cmd.Qty, cmd.ReceivedBy);
        await db.SaveChangesAsync(ct);

        foreach (var ev in product.DomainEvents)
            await mediator.Publish(ev, ct);
        product.ClearEvents();
    }
}
