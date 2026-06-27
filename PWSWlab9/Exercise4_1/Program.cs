using KanbanSystem.Application.Cards;
using KanbanSystem.Application.Interfaces;
using KanbanSystem.Domain;
using KanbanSystem.Domain.Entities;
using KanbanSystem.Domain.ValueObjects;
using KanbanSystem.Infrastructure.Persistence;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("=== Cwiczenie 4.1: Clean Architecture - system Kanban ===\n");

var services = new ServiceCollection();
services.AddLogging();
services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateCardHandler).Assembly));
services.AddSingleton<InMemoryCardRepository>();
services.AddSingleton<ICardRepository>(sp => sp.GetRequiredService<InMemoryCardRepository>());
services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();

var provider = services.BuildServiceProvider();
var mediator = provider.GetRequiredService<IMediator>();
var repo = provider.GetRequiredService<InMemoryCardRepository>();
var uow = (InMemoryUnitOfWork)provider.GetRequiredService<IUnitOfWork>();

var columnId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

Console.WriteLine("--- CreateCard przez MediatR ---");
var wynik = await mediator.Send(new CreateCardCommand(
    "Implementacja API", PriorityLevel: 3, ColumnId: columnId));

Console.WriteLine($"  Utworzono karte: {wynik.CardId}");
Console.WriteLine($"  Data utworzenia: {wynik.CreatedAt:u}");

var karta = await repo.GetByIdAsync(wynik.CardId);
Console.WriteLine($"  Tytul: {karta!.Title}, Priorytet: {karta.Priority.Name}");
Console.WriteLine($"  SaveChanges wywolane: {uow.SaveCount} razy");

Console.WriteLine("\n--- Walidacja domenowa ---");
try
{
    Card.Create("", CardPriority.Low, columnId);
    Console.WriteLine("  BLAD: powinien zostac rzucony wyjatek");
}
catch (DomainException ex)
{
    Console.WriteLine($"  DomainException: {ex.Message}");
}

try
{
    CardPriority.FromLevel(99);
    Console.WriteLine("  BLAD: powinien zostac rzucony wyjatek");
}
catch (DomainException ex)
{
    Console.WriteLine($"  DomainException: {ex.Message}");
}

Console.WriteLine("\n=== Koniec cwiczenia 4.1 ===");
