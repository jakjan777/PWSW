namespace Exercise3_1.Pipeline;

public class BuildPipeline
{
    private readonly List<IBuildStep> _steps = [];

    public BuildPipeline AddStep(IBuildStep step)
    {
        _steps.Add(step);
        return this;
    }

    public async Task ExecuteAsync(BuildContext context)
    {
        var index = 0;
        async Task Next()
        {
            if (index < _steps.Count)
            {
                var step = _steps[index++];
                await step.ExecuteAsync(context, Next);
            }
        }
        await Next();
    }
}
