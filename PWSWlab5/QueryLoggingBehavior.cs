namespace PWSW5;

public class QueryLoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly List<string> _log;

    public QueryLoggingBehavior(List<string> log) => _log = log;

    public async Task<TResponse> HandleAsync(
        TRequest request,
        Func<Task<TResponse>> next,
        CancellationToken ct)
    {
        if (request is HrQuery hrQuery)
            _log.Add($"[{DateTime.Now:HH:mm:ss}] Pytanie: {hrQuery.Question}");

        var result = await next();

        if (result is HrResponse hrResponse)
        {
            var skrot = hrResponse.Answer.Length > 80
                ? hrResponse.Answer[..80] + "..."
                : hrResponse.Answer;
            _log.Add($"[{DateTime.Now:HH:mm:ss}] Znaleziono={hrResponse.FoundInKnowledgeBase}, odpowiedz: {skrot}");
        }

        return result;
    }
}
