namespace Exercise3_1.Pipeline;

public class RestoreStep : IBuildStep
{
    public Task ExecuteAsync(BuildContext ctx, Func<Task> next)
    {
        ctx.Log.Add("[RESTORE] Przywracanie zaleznosci...");
        ctx.Log.Add("[RESTORE] OK.");
        return next();
    }
}
