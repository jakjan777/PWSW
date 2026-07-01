using BibliotekaKatalog;

var library = new LibrarySection
{
    Name = "Glowna",
    Books =
    [
        new Book("978-1", "Solaris", "Lem", 1961, Genre.Fiction, 204),
        new Book("978-2", "Kosmos", "Lem", 1959, Genre.Science, 312)
    ],
    SubSections =
    [
        new LibrarySection
        {
            Name = "Historia",
            Books =
            [
                new Book("978-3", "Bitwa pod Grunwaldem", "Davies", 2005, Genre.History, 480),
                new Book("978-4", "Krzyzacy", "Sienkiewicz", 1900, Genre.Fiction, 650)
            ]
        },
        new LibrarySection
        {
            Name = "Technologia",
            Books =
            [
                new Book("978-5", "C# in Depth", "Skeet", 2019, Genre.Technology, 528),
                new Book("978-6", "Clean Code", "Martin", 2008, Genre.Technology, 464)
            ],
            SubSections =
            [
                new LibrarySection
                {
                    Name = "Dziecieca",
                    Books =
                    [
                        new Book("978-7", "Harry Potter", "Rowling", 1997, Genre.Children, 320)
                    ]
                }
            ]
        }
    ]
};

Console.WriteLine("=== BookCatalog z yield return ===");
var catalog = new BookCatalog();
catalog.Add(new Book("978-0", "Dune", "Herbert", 1965, Genre.Fiction, 412));
catalog.Add(new Book("978-8", "Fundacja", "Asimov", 1951, Genre.Science, 255));
foreach (var book in catalog)
    Console.WriteLine($"  {book.Title} ({book.Pages} str.)");

Console.WriteLine();
Console.WriteLine("=== Rekurencyjny SectionIterator ===");
foreach (var (section, book) in SectionIterator.TraverseAll(library))
    Console.WriteLine($"  [{section}] {book.Title}");

Console.WriteLine();
Console.WriteLine("=== Filtry lancuchowe: Fiction >= 300 str. ===");
var allBooks = SectionIterator.TraverseAll(library).Select(x => x.Book);
var filtered = BookFilters.ByMinPages(
    BookFilters.ByGenre(allBooks, Genre.Fiction), 300);

foreach (var book in filtered)
    Console.WriteLine($"  {book.Title} ({book.Pages} str.)");

Console.WriteLine();
Console.WriteLine("=== Grupowanie po gatunku ===");
var stats = SectionIterator.TraverseAll(library)
    .GroupBy(x => x.Book.Genre)
    .Select(g => new { Genre = g.Key, Count = g.Count() })
    .OrderByDescending(g => g.Count);

foreach (var stat in stats)
    Console.WriteLine($"  {stat.Genre}: {stat.Count} ksiazek");
