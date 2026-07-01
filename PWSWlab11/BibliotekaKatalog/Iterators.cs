using System.Collections;

namespace BibliotekaKatalog;

public class BookCatalog : IEnumerable<Book>
{
    private readonly List<Book> _books = [];

    public void Add(Book book) => _books.Add(book);

    public IEnumerator<Book> GetEnumerator()
    {
        for (int i = 0; i < _books.Count; i++)
            yield return _books[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static class SectionIterator
{
    public static IEnumerable<(string Section, Book Book)> TraverseAll(
        LibrarySection section, string path = "")
    {
        string current = string.IsNullOrEmpty(path)
            ? section.Name
            : $"{path} > {section.Name}";

        foreach (var book in section.Books)
            yield return (current, book);

        foreach (var sub in section.SubSections)
        foreach (var item in TraverseAll(sub, current))
            yield return item;
    }
}

public static class BookFilters
{
    public static IEnumerable<Book> ByGenre(IEnumerable<Book> books, Genre genre)
    {
        foreach (var book in books)
            if (book.Genre == genre)
                yield return book;
    }

    public static IEnumerable<Book> ByMinPages(IEnumerable<Book> books, int min)
    {
        foreach (var book in books)
            if (book.Pages >= min)
                yield return book;
    }
}
