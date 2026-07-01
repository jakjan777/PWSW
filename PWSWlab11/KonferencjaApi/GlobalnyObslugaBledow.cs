using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace KonferencjaApi;

public class GlobalnyObslugaBledow : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Wystapil nieoczekiwany blad serwera",
            Detail = "Skontaktuj sie z administratorem podajac TraceId",
            Instance = httpContext.Request.Path,
            Extensions = { ["traceId"] = httpContext.TraceIdentifier }
        };

        httpContext.Response.StatusCode = problem.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
