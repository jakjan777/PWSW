namespace Warehouse.Domain;

public class WarehouseProduct
{
    public Guid Id { get; private set; }
    public string Sku { get; private set; } = "";
    public string Name { get; private set; } = "";
    public string Category { get; private set; } = "";
    public int QuantityOnHand { get; private set; }
    public int ReorderLevel { get; private set; }

    private readonly List<IDomainEvent> _events = [];
    public IReadOnlyList<IDomainEvent> DomainEvents => _events.AsReadOnly();
    public void ClearEvents() => _events.Clear();

    private WarehouseProduct() { }

    public static WarehouseProduct Create(
        string sku, string name, string category, int qty, int reorder)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        ArgumentOutOfRangeException.ThrowIfNegative(qty);

        var p = new WarehouseProduct
        {
            Id = Guid.NewGuid(),
            Sku = sku,
            Name = name,
            Category = category,
            QuantityOnHand = qty,
            ReorderLevel = reorder
        };
        p._events.Add(new ProductCreatedEvent(p.Id, sku, name));
        return p;
    }

    public void ReceiveStock(int qty, string receivedBy)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(qty);
        QuantityOnHand += qty;
        _events.Add(new StockReceivedEvent(
            Id, qty, QuantityOnHand, receivedBy));
    }

    public void IssueStock(int qty, string issuedBy, string reason)
    {
        if (qty > QuantityOnHand)
            throw new InvalidOperationException(
                $"Brak wystarczajacej ilosci. Dostepne: {QuantityOnHand}, zadane: {qty}");
        QuantityOnHand -= qty;
        _events.Add(new StockIssuedEvent(
            Id, qty, QuantityOnHand, issuedBy, reason));
        if (QuantityOnHand <= ReorderLevel)
            _events.Add(new LowStockAlertEvent(
                Id, Sku, Name, QuantityOnHand, ReorderLevel));
    }
}
