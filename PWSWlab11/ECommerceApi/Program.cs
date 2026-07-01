using ECommerceApi.Contracts.Responses;
using ECommerceApi.Domain;
using ECommerceApi.Mapping;
using Mapster;

MapsterConfig.Configure();

var products = new List<Product>
{
    new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111101"), Name = "Laptop Pro", Description = "15 cali", CostPrice = 4000m, Margin = 1500m, StockQuantity = 12, Sku = "LAP-001" },
    new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111102"), Name = "Mysz BT", Description = "Ergonomiczna", CostPrice = 80m, Margin = 40m, StockQuantity = 50, Sku = "MOU-002" },
    new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111103"), Name = "Monitor 27", Description = "4K IPS", CostPrice = 1200m, Margin = 400m, StockQuantity = 0, Sku = "MON-003" },
    new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111104"), Name = "Klawiatura", Description = "Mechaniczna", CostPrice = 250m, Margin = 100m, StockQuantity = 30, Sku = "KEY-004" },
    new() { Id = Guid.Parse("11111111-1111-1111-1111-111111111105"), Name = "Hub USB-C", Description = "7 portow", CostPrice = 120m, Margin = 60m, StockQuantity = 25, Sku = "HUB-005" }
};

var customer = new Customer
{
    FirstName = "Anna",
    LastName = "Kowalska",
    Email = "anna@firma.pl",
    PhoneNumber = "+48 600 100 200",
    PasswordHash = "bcrypt$hashed_secret"
};

var order = new Order
{
    Id = Guid.Parse("22222222-2222-2222-2222-222222222201"),
    Customer = customer,
    Status = OrderStatus.Confirmed,
    Items =
    [
        new OrderItem { Product = products[0], Quantity = 1, UnitPrice = products[0].SellingPrice },
        new OrderItem { Product = products[1], Quantity = 2, UnitPrice = products[1].SellingPrice }
    ]
};

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/products", (int page = 1, int pageSize = 2) =>
{
    var total = products.Count;
    var pageItems = products
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(p => p.ToResponse())
        .ToList();

    return Results.Ok(new PagedResponse<ProductResponse>(pageItems, total, page, pageSize));
});

app.MapGet("/products/{id:guid}", (Guid id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    return product is null
        ? Results.NotFound()
        : Results.Ok(product.ToResponse());
});

app.MapGet("/orders/{id:guid}/manual", (Guid id) =>
{
    if (order.Id != id) return Results.NotFound();
    return Results.Ok(order.ToResponse());
});

app.MapGet("/orders/{id:guid}/mapster", (Guid id) =>
{
    if (order.Id != id) return Results.NotFound();
    return Results.Ok(order.Adapt<OrderResponse>());
});

app.Run();
