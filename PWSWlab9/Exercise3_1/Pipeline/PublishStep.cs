namespace Exercise3_1.Pipeline;

public class PublishStep : IBuildStep
{
    public Task ExecuteAsync(BuildContext ctx, Func<Task> next)
    {
        ctx.OutputPath = $"./publish-{ctx.Runtime}";
        ctx.Log.Add($"[PUBLISH] Artefakt: {ctx.OutputPath}");
        return next();
    }
}
