using NSubstitute;
using Xunit;

namespace Challenge1.Tests;

public class DockerServiceProxyTests
{
    [Fact]
    public async Task ListContainersAsync_BezTokenu_RzucaUnauthorized()
    {
        var real = Substitute.For<IDockerService>();
        var proxy = new DockerServiceProxy(real, "");

        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => proxy.ListContainersAsync());
    }

    [Fact]
    public async Task ListContainersAsync_DrugieWywolanie_UzywaCache()
    {
        var real = new FakeDockerService();
        var proxy = new DockerServiceProxy(real, "secret-token");

        var first = (await proxy.ListContainersAsync()).ToList();
        var second = (await proxy.ListContainersAsync()).ToList();

        Assert.Equal(first, second);
        Assert.Equal(1, real.ListCallCount);
    }

    [Fact]
    public async Task StartContainerAsync_UniewazniaCache()
    {
        var real = new FakeDockerService();
        var proxy = new DockerServiceProxy(real, "secret-token");

        await proxy.ListContainersAsync();
        await proxy.StartContainerAsync("web-1");
        await proxy.ListContainersAsync();

        Assert.Equal(2, real.ListCallCount);
    }

    [Fact]
    public async Task GetContainerLogsAsync_ZawszeDeleguje()
    {
        var real = Substitute.For<IDockerService>();
        real.GetContainerLogsAsync("web-1").Returns("log line");
        var proxy = new DockerServiceProxy(real, "secret-token");

        var logs = await proxy.GetContainerLogsAsync("web-1");

        Assert.Equal("log line", logs);
        await real.Received(1).GetContainerLogsAsync("web-1");
    }
}
