namespace Exercise3_1.Pipeline;

public class BuildStep : IBuildStep
{
    public Task ExecuteAsync(BuildContext ctx, Func<Task> next)
    {
        ctx.Log.Add("[BUILD] Kompilacja...");
        if (string.IsNullOrEmpty(ctx.ProjectPath))
        {
            ctx.Success = false;
            ctx.ErrorMessage = "Brak sciezki projektu";
            return Task.CompletedTask;
        }
        ctx.Log.Add("[BUILD] OK.");
        return next();
    }
}
