using System.ComponentModel.DataAnnotations;
using System.Data;
using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Warehouse.Application.Commands;
using Warehouse.Application.Queries;
using Warehouse.Domain;
using Warehouse.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("WriteDb")
    ?? throw new InvalidOperationException(
        "Brak connection string WriteDb w konfiguracji!");

builder.Services.AddDbContext<WriteDbContext>(opt =>
    opt.UseSqlite(connectionString));
builder.Services.AddScoped<IDbConnection>(_ => new SqliteConnection(connectionString));
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var host = builder.Build();

using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<WriteDbContext>();
await db.Database.EnsureDeletedAsync();
await db.Database.EnsureCreatedAsync();

var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

var laptop = WarehouseProduct.Create("LAP-001", "Laptop Dell", "Elektronika", 20, 10);
var phone = WarehouseProduct.Create("PHN-002", "Telefon Samsung", "Elektronika", 50, 15);

db.Products.AddRange(laptop, phone);
await db.SaveChangesAsync();

Console.WriteLine("=== Wyzwanie 2: SecureIssueStockCommand ===");
await mediator.Send(new SecureIssueStockCommand(
    laptop.Id, 3, "Anna K.", "Wydanie do klienta", "192.168.1.10"));
await mediator.Send(new SecureIssueStockCommand(
    phone.Id, 5, "Jan N.", "Reklamacja", "10.0.0.5"));

Console.WriteLine();
Console.WriteLine("=== Walidacja odrzucona ===");
try
{
    await mediator.Send(new SecureIssueStockCommand(
        laptop.Id, 0, "", "", "127.0.0.1"));
}
catch (ValidationException ex)
{
    Console.WriteLine($"BLAD: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("=== GetAuditLogQuery (Dapper, strona 1) ===");
var auditPage = await mediator.Send(new GetAuditLogQuery(1, 10));
foreach (var entry in auditPage)
    Console.WriteLine(
        $"{entry.OccurredAt:yyyy-MM-dd HH:mm:ss} | {entry.UserId,-10} | " +
        $"{entry.Action,-12} | {entry.IpAddress,-15} | {entry.Details}");

Console.WriteLine();
Console.WriteLine("=== GetProductsQuery ===");
var products = await mediator.Send(new GetProductsQuery(null));
foreach (var p in products)
    Console.WriteLine($"{p.Sku,-10} {p.Name,-20} {p.QuantityOnHand,4} szt. [{p.StockStatus}]");
