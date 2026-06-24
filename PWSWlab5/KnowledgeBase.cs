namespace PWSW5;

public class KnowledgeBase
{
    private readonly List<DocumentChunk> _chunks = [];

    public int ChunkCount => _chunks.Count;

    public void LoadFromDirectory(string path)
    {
        foreach (var file in Directory.GetFiles(path, "*.txt"))
        {
            var content = File.ReadAllText(file);
            var fileName = Path.GetFileName(file);
            var chunks = ChunkText(content, maxChunkSize: 500);

            for (int i = 0; i < chunks.Length; i++)
                _chunks.Add(new DocumentChunk(
                    Id: $"{fileName}_chunk_{i}",
                    Content: chunks[i],
                    Source: fileName));

            Console.WriteLine($"Zaladowano: {fileName} ({chunks.Length} fragmentow)");
        }
        Console.WriteLine($"Lacznie: {_chunks.Count} fragmentow w bazie.");
    }

    public DocumentChunk[] Search(string query, int topK = 3)
    {
        var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "jaka", "jaki", "jakie", "jest", "czy", "co", "na", "za", "w", "i", "o", "do", "po", "od"
        };

        var terms = query.ToLower()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(t => !stopWords.Contains(t))
            .ToArray();

        if (terms.Length == 0)
            return [];

        return _chunks
            .Select(c => (Chunk: c, Score: terms.Count(t =>
                c.Content.Contains(t, StringComparison.OrdinalIgnoreCase))))
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .Take(topK)
            .Select(x => x.Chunk)
            .ToArray();
    }

    private static string[] ChunkText(string text, int maxChunkSize)
    {
        var sentences = text.Split('.',
            StringSplitOptions.RemoveEmptyEntries);
        var chunks = new List<string>();
        var current = "";

        foreach (var sentence in sentences)
        {
            if (current.Length + sentence.Length > maxChunkSize && current.Length > 0)
            {
                chunks.Add(current.Trim());
                current = "";
            }
            current += sentence.Trim() + ". ";
        }

        if (current.Trim().Length > 0)
            chunks.Add(current.Trim());

        return chunks.ToArray();
    }
}
