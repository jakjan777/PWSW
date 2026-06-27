namespace Challenge1;

public class FakeDockerService : IDockerService
{
    private readonly List<string> _containers = ["web-1", "db-1", "adminer-1"];
    private int _listCallCount;

    public int ListCallCount => _listCallCount;

    public Task<IEnumerable<string>> ListContainersAsync()
    {
        _listCallCount++;
        return Task.FromResult<IEnumerable<string>>(_containers.ToList());
    }

    public Task<string> GetContainerLogsAsync(string containerId) =>
        Task.FromResult($"Logi kontenera {containerId}: OK");

    public Task StartContainerAsync(string containerId)
    {
        if (!_containers.Contains(containerId))
            throw new InvalidOperationException($"Kontener {containerId} nie istnieje.");

        return Task.CompletedTask;
    }
}
