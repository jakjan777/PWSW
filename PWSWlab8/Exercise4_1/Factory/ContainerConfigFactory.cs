namespace Exercise4_1.Factory;

public class ContainerConfigFactory
{
    private readonly Dictionary<string, Func<string, ContainerConfig>> _creators = [];

    public record ContainerConfig(
        string ImageName,
        int MemoryMB,
        int CpuShares,
        Dictionary<string, string> EnvVars);

    public ContainerConfigFactory Register(
        string env, Func<string, ContainerConfig> creator)
    {
        _creators[env] = creator;
        return this;
    }

    public ContainerConfig Create(string env, string app)
    {
        if (!_creators.TryGetValue(env, out var creator))
            throw new InvalidOperationException(
                $"Brak konfiguracji dla srodowiska: {env}");

        return creator(app);
    }
}
