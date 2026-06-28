using System.Collections.Concurrent;
using System.Threading.Channels;
using OrderPipeline;

var chValidation = Channel.CreateBounded<Order>(new BoundedChannelOptions(5)
{
    FullMode = BoundedChannelFullMode.Wait,
    SingleWriter = true,
    SingleReader = false
});
var chPayment = Channel.CreateBounded<Order>(5);
var chShipping = Channel.CreateBounded<Order>(5);

var statuses = new ConcurrentDictionary<string, OrderStatus>();

var stock = new ConcurrentDictionary<string, int>(new Dictionary<string, int>
{
    ["Laptop"] = 10,
    ["Phone"] = 25,
    ["Tablet"] = 0
});

async Task ValidateAsync(CancellationToken ct)
{
    await foreach (var order in chValidation.Reader.ReadAllAsync(ct))
    {
        await Task.Delay(100, ct);
        if (stock.TryGetValue(order.Product, out int qty) && qty >= order.Qty)
        {
            stock.AddOrUpdate(order.Product, 0, (_, s) => s - order.Qty);
            order.Status = OrderStatus.Validated;
            statuses[order.Id] = OrderStatus.Validated;
            await chPayment.Writer.WriteAsync(order, ct);
            Console.WriteLine($"[Walidacja] {order.Id} OK");
        }
        else
        {
            order.Status = OrderStatus.Error;
            order.ErrorMsg = $"Brak {order.Product} (stan: {qty})";
            statuses[order.Id] = OrderStatus.Error;
            Console.WriteLine($"[Walidacja] {order.Id} BLAD");
        }
    }
    chPayment.Writer.Complete();
}

async Task PayAsync(CancellationToken ct)
{
    using var sem = new SemaphoreSlim(2);
    var tasks = new List<Task>();
    await foreach (var order in chPayment.Reader.ReadAllAsync(ct))
    {
        await sem.WaitAsync(ct);
        tasks.Add(Task.Run(async () =>
        {
            try
            {
                await Task.Delay(200, ct);
                order.Status = OrderStatus.Paid;
                statuses[order.Id] = OrderStatus.Paid;
                await chShipping.Writer.WriteAsync(order, ct);
                Console.WriteLine($"[Platnosc] {order.Id} OK");
            }
            finally { sem.Release(); }
        }, ct));
    }
    await Task.WhenAll(tasks);
    chShipping.Writer.Complete();
}

async Task ShipAsync(CancellationToken ct)
{
    int num = 1;
    await foreach (var order in chShipping.Reader.ReadAllAsync(ct))
    {
        await Task.Delay(150, ct);
        order.Status = OrderStatus.Shipped;
        statuses[order.Id] = OrderStatus.Shipped;
        Console.WriteLine($"[Wysylka] {order.Id} -> LP-{num++:D6}");
    }
}

using var cts = new CancellationTokenSource();
var taskV = Task.Run(() => ValidateAsync(cts.Token));
var taskP = Task.Run(() => PayAsync(cts.Token));
var taskS = Task.Run(() => ShipAsync(cts.Token));

Order[] orders =
[
    new() { Id = "ZAM-001", Client = "Anna K.", Product = "Laptop", Qty = 1, Price = 3999m },
    new() { Id = "ZAM-002", Client = "Jan N.", Product = "Phone", Qty = 2, Price = 1499m },
    new() { Id = "ZAM-003", Client = "Ewa W.", Product = "Tablet", Qty = 1, Price = 2199m },
];

foreach (var o in orders)
{
    statuses[o.Id] = OrderStatus.New;
    await chValidation.Writer.WriteAsync(o);
}
chValidation.Writer.Complete();

await Task.WhenAll(taskV, taskP, taskS);

Console.WriteLine();
Console.WriteLine("=== RAPORT ===");
foreach (var (id, status) in statuses.OrderBy(s => s.Key))
    Console.WriteLine($"{id}: {status}");
