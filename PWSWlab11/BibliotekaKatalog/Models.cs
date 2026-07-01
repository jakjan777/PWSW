namespace BibliotekaKatalog;

public enum Genre { Fiction, Science, History, Children, Technology }

public record Book(string Isbn, string Title, string Author, int Year, Genre Genre, int Pages);

public class LibrarySection
{
    public string Name { get; init; } = string.Empty;
    public List<Book> Books { get; init; } = [];
    public List<LibrarySection> SubSections { get; init; } = [];
}
