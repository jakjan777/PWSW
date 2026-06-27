namespace Challenge1;

public class DockerServiceProxy : IDockerService
{
    private readonly IDockerService _real;
    private readonly string _authToken;

    private readonly Dictionary<string, (DateTime Cached, IEnumerable<string> Data)> _cache = [];

    public DockerServiceProxy(IDockerService real, string authToken)
    {
        _real = real;
        _authToken = authToken;
    }

    private void CheckAuth()
    {
        if (string.IsNullOrEmpty(_authToken))
            throw new UnauthorizedAccessException("Brak tokenu uwierzytelniajacego.");
    }

    public async Task<IEnumerable<string>> ListContainersAsync()
    {
        CheckAuth();
        const string cacheKey = "containers";

        if (_cache.TryGetValue(cacheKey, out var entry)
            && DateTime.Now - entry.Cached < TimeSpan.FromMinutes(5))
        {
            return entry.Data;
        }

        var wynik = await _real.ListContainersAsync();
        _cache[cacheKey] = (DateTime.Now, wynik);
        return wynik;
    }

    public async Task<string> GetContainerLogsAsync(string containerId)
    {
        CheckAuth();
        return await _real.GetContainerLogsAsync(containerId);
    }

    public async Task StartContainerAsync(string containerId)
    {
        CheckAuth();
        await _real.StartContainerAsync(containerId);
        _cache.Remove("containers");
    }
}
