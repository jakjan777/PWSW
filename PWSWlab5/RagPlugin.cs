using System.ComponentModel;
using System.Text;
using Microsoft.SemanticKernel;

namespace PWSW5;

public class RagPlugin(KnowledgeBase kb)
{
    [KernelFunction, Description("Wyszukuje informacje w bazie wiedzy firmy")]
    public string SearchKnowledge(
        [Description("Zapytanie do wyszukania")] string query)
    {
        var results = kb.Search(query, topK: 3);
        if (results.Length == 0)
            return "Nie znaleziono informacji w bazie wiedzy.";

        var sb = new StringBuilder("Znalezione fragmenty:\n");
        foreach (var chunk in results)
        {
            sb.AppendLine($"[Zrodlo: {chunk.Source}]");
            sb.AppendLine(chunk.Content);
            sb.AppendLine("---");
        }
        return sb.ToString();
    }
}
