namespace Challenge1;

public interface IDockerService
{
    Task<IEnumerable<string>> ListContainersAsync();
    Task<string> GetContainerLogsAsync(string containerId);
    Task StartContainerAsync(string containerId);
}
