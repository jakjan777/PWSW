namespace Challenge1;

public static class SigningStep
{
    public static Func<BuildDelegate, BuildDelegate> Create() =>
        next => async ctx =>
        {
            if (!ctx.Artifacts.ContainsKey("msix"))
            {
                ctx.Success = false;
                ctx.Log.Add("[Signing] Brak artefaktu MSIX do podpisania");
                return;
            }

            ctx.Log.Add($"[Signing] Podpisywanie {ctx.Artifacts["msix"]}...");
            await next(ctx);
            ctx.Log.Add("[Signing] Podpisano pomyslnie");
        };
}
