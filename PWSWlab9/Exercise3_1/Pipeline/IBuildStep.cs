namespace Exercise3_1.Pipeline;

public interface IBuildStep
{
    Task ExecuteAsync(BuildContext context, Func<Task> next);
}
