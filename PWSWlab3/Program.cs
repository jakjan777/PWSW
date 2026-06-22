using Microsoft.AspNetCore.Http.HttpResults;
using PWSWlab3.Filters;
using PWSWlab3.Models;
using PWSWlab3.Services;

if (args.Contains("--demo-1-1"))
{
    RunExercise11Demo();
    return;
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<InMemoryNoteRepository>();
builder.Services.AddSingleton<NoteService>();
builder.Services.AddSingleton<DecoratorDiagnostics>();
builder.Services.AddSingleton<INoteService>(sp =>
{
    var baseService = sp.GetRequiredService<NoteService>();
    var logger = sp.GetRequiredService<ILogger<LoggingNoteDecorator>>();
    var diagnostics = sp.GetRequiredService<DecoratorDiagnostics>();
    var logging = new LoggingNoteDecorator(baseService, logger);
    return new CachingNoteDecorator(logging, diagnostics);
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.MapGet("/", () => "PWSWlab3 LAB-03: uruchom /api/notes, aby zobaczyc endpointy notatek.");

var notes = app.MapGroup("/api/notes")
    .WithTags("Notatki")
    .AddEndpointFilter<FiltrLogowania>();

notes.MapGet("/", GetNotes)
    .WithName("GetNotes");

notes.MapGet("/{id:guid}", GetNoteById)
    .WithName("GetNoteById");

notes.MapPost("/", CreateNote)
    .WithName("CreateNote");

notes.MapPut("/{id:guid}", UpdateNote)
    .WithName("UpdateNote");

notes.MapDelete("/{id:guid}", DeleteNote)
    .WithName("DeleteNote");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Notes}/{action=Index}/{id?}");

app.Run();

static Results<Ok<NotesPageResponse>, BadRequest<ErrorResponse>> GetNotes(
    INoteService service,
    string? q,
    int page = 1,
    int size = 10)
{
    if (page < 1 || size < 1)
    {
        return TypedResults.BadRequest(new ErrorResponse("Parametry page i size musza byc wieksze od zera."));
    }

    var all = string.IsNullOrWhiteSpace(q)
        ? service.GetAll().ToList()
        : service.Search(q).ToList();

    var data = all
        .Skip((page - 1) * size)
        .Take(size)
        .ToList();

    return TypedResults.Ok(new NotesPageResponse(data, page, size, all.Count));
}

static Results<Ok<Note>, NotFound<ErrorResponse>> GetNoteById(Guid id, INoteService service)
{
    return service.GetById(id) is { } note
        ? TypedResults.Ok(note)
        : TypedResults.NotFound(new ErrorResponse($"Notatka {id} nie istnieje."));
}

static Results<Created<Note>, BadRequest<ErrorResponse>> CreateNote(Note note, INoteService service)
{
    if (string.IsNullOrWhiteSpace(note.Title))
    {
        return TypedResults.BadRequest(new ErrorResponse("Tytul notatki jest wymagany."));
    }

    service.Add(note);
    return TypedResults.Created($"/api/notes/{note.Id}", note);
}

static Results<Ok<Note>, BadRequest<ErrorResponse>, NotFound<ErrorResponse>> UpdateNote(
    Guid id,
    Note note,
    INoteService service)
{
    if (string.IsNullOrWhiteSpace(note.Title))
    {
        return TypedResults.BadRequest(new ErrorResponse("Tytul notatki jest wymagany."));
    }

    note.Id = id;

    return service.Update(note)
        ? TypedResults.Ok(note)
        : TypedResults.NotFound(new ErrorResponse($"Notatka {id} nie istnieje."));
}

static Results<NoContent, NotFound<ErrorResponse>> DeleteNote(Guid id, INoteService service)
{
    return service.Delete(id)
        ? TypedResults.NoContent()
        : TypedResults.NotFound(new ErrorResponse($"Notatka {id} nie istnieje."));
}

static void RunExercise11Demo()
{
    var repository = InMemoryNoteRepository.Create();
    foreach (var note in repository.GetAll())
    {
        repository.Delete(note.Id);
    }

    var notes = new[]
    {
        new Note
        {
            Title = "Raport",
            Content = "Przygotowac raport z laboratorium.",
            Category = "Praca",
            IsPinned = true
        },
        new Note
        {
            Title = "Wakacje",
            Content = "Lista rzeczy do spakowania.",
            Category = "Osobiste"
        },
        new Note
        {
            Title = "Pomysl",
            Content = new string('x', 600),
            Category = "Pomysly"
        },
        new Note
        {
            Title = "Pusta",
            Content = string.Empty
        },
        new Note
        {
            Title = "Moj pomysl",
            Content = "Krotka notatka z pomyslem bez kategorii."
        },
        new Note
        {
            Title = "Ogolna",
            Content = "Zwykla notatka bez specjalnych cech."
        }
    };

    foreach (var note in notes)
    {
        repository.Add(note);
    }

    Console.WriteLine("=== Klasyfikacja notatek ===");
    foreach (var note in notes)
    {
        Console.WriteLine($"[{note.Title}] -> {NoteClassifier.Classify(note)}");
    }

    Console.WriteLine();
    Console.WriteLine("=== Wyszukiwanie (DIM) ===");
    foreach (var note in ((ISearchable)repository).Search("pomysl"))
    {
        Console.WriteLine($"Znaleziono: {note.Title}");
    }

    Console.WriteLine();
    Console.WriteLine("=== Analiza aktywnosci (list patterns) ===");
    PrintActivity([]);
    PrintActivity([5]);
    PrintActivity([15, 12, 3, 4]);
    PrintActivity([3, 7, 9, 12]);
    PrintActivity([5, 3, 2]);

    static void PrintActivity(int[] notesPerDay)
    {
        Console.WriteLine($"[{string.Join(", ", notesPerDay)}] -> {NoteClassifier.AnalyzeActivity(notesPerDay)}");
    }
}

public sealed record NotesPageResponse(
    IReadOnlyList<Note> Data,
    int Page,
    int Size,
    int TotalCount);

public sealed record ErrorResponse(string Error);
