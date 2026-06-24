using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace PWSW5;

public class HrAssistantPlugin(KnowledgeBase kb)
{
    [KernelFunction]
    [Description("Odpowiada na pytania pracownikow na podstawie bazy wiedzy HR")]
    public string AnswerHrQuestion(
        [Description("Pytanie pracownika")] string question)
    {
        var results = kb.Search(question, topK: 2);
        if (results.Length == 0)
            return "Brak danych w bazie wiedzy HR. Nie moge udzielic odpowiedzi na to pytanie.";

        var odpowiedz = "Na podstawie dokumentow firmy:\n";
        foreach (var chunk in results)
            odpowiedz += $"- [{chunk.Source}] {chunk.Content.Trim()}\n";

        return odpowiedz.Trim();
    }
}
