namespace Challenge1;

public class BuildPipeline
{
    private readonly List<Func<BuildDelegate, BuildDelegate>> _middlewares = [];

    public BuildPipeline Use(Func<BuildDelegate, BuildDelegate> middleware)
    {
        _middlewares.Add(middleware);
        return this;
    }

    public async Task Run(BuildContext context)
    {
        BuildDelegate terminal = _ => Task.CompletedTask;

        var current = terminal;
        for (var i = _middlewares.Count - 1; i >= 0; i--)
            current = _middlewares[i](current);

        await current(context);
    }
}
