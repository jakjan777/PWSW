using Microsoft.AspNetCore.Mvc;
using PWSWlab3.Models;
using PWSWlab3.Services;
using PWSWlab3.ViewModels;

namespace PWSWlab3.Controllers;

[Route("notes")]
public class NotesController(
    INoteService service,
    DecoratorDiagnostics diagnostics) : Controller
{
    private const int PageSize = 20;

    [HttpGet("")]
    public IActionResult Index(string? q, int page = 1)
    {
        page = Math.Max(page, 1);

        var all = service.GetAll();
        var filtered = string.IsNullOrWhiteSpace(q)
            ? all
            : service.Search(q).ToList();

        var vm = new NoteListViewModel
        {
            Notes = filtered
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .Select(note => new NoteSummary
                {
                    Id = note.Id,
                    Title = note.Title,
                    Category = note.Category,
                    IsPinned = note.IsPinned,
                    ModifiedAt = note.ModifiedAt
                })
                .ToList(),
            SearchQuery = q,
            TotalCount = filtered.Count,
            CurrentPage = page,
            DecoratorStatus = diagnostics.LastCacheEvent
        };

        return View(vm);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View(new NoteFormViewModel());
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(NoteFormViewModel form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        service.Add(new Note
        {
            Title = form.Title,
            Content = form.Content ?? string.Empty,
            Category = form.Category,
            IsPinned = form.IsPinned
        });

        TempData["Success"] = "Notatka zostala dodana.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("edit/{id:guid}")]
    public IActionResult Edit(Guid id)
    {
        var note = service.GetById(id);
        if (note is null)
        {
            return NotFound();
        }

        return View(new NoteFormViewModel
        {
            Id = note.Id,
            Title = note.Title,
            Content = note.Content,
            Category = note.Category,
            IsPinned = note.IsPinned
        });
    }

    [HttpPost("edit/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Guid id, NoteFormViewModel form)
    {
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var existing = service.GetById(id);
        if (existing is null)
        {
            return NotFound();
        }

        var updated = new Note
        {
            Id = id,
            Title = form.Title,
            Content = form.Content ?? string.Empty,
            Category = form.Category,
            IsPinned = form.IsPinned,
            CreatedAt = existing.CreatedAt
        };

        service.Update(updated);

        TempData["Success"] = "Notatka zostala zaktualizowana.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("delete/{id:guid}")]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(Guid id)
    {
        service.Delete(id);

        TempData["Success"] = "Notatka zostala usunieta.";
        return RedirectToAction(nameof(Index));
    }
}
