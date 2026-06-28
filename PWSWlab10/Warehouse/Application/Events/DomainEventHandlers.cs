using MediatR;
using Warehouse.Domain;

namespace Warehouse.Application.Events;

public class StockReceivedEventHandler
    : INotificationHandler<StockReceivedEvent>
{
    public Task Handle(StockReceivedEvent ev, CancellationToken ct)
    {
        Console.WriteLine(
            $"[ReadModel] Przyjeto {ev.Qty} szt. produktu {ev.ProductId}, stan: {ev.NewTotal}");
        return Task.CompletedTask;
    }
}

public class LowStockAlertEventHandler
    : INotificationHandler<LowStockAlertEvent>
{
    public Task Handle(LowStockAlertEvent ev, CancellationToken ct)
    {
        Console.WriteLine(
            $"[ReadModel] ALERT niski stan: {ev.Sku} ({ev.Name}) = {ev.CurrentQty}");
        return Task.CompletedTask;
    }
}
