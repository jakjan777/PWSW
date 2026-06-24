namespace PWSW5;

public class HrQueryHandler(HrAssistantPlugin plugin) : IRequestHandler<HrQuery, HrResponse>
{
    public Task<HrResponse> HandleAsync(HrQuery request, CancellationToken ct = default)
    {
        var answer = plugin.AnswerHrQuestion(request.Question);
        var found = !answer.StartsWith("Brak danych", StringComparison.OrdinalIgnoreCase);
        return Task.FromResult(new HrResponse(answer, found));
    }
}
