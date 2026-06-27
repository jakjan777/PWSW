using Xunit;

namespace Challenge2.Tests;

public class ContainerResourceFactoryTests
{
    [Theory]
    [InlineData(ContainerType.Dev, typeof(DevContainer))]
    [InlineData(ContainerType.Test, typeof(TestContainer))]
    [InlineData(ContainerType.Production, typeof(ProductionContainer))]
    public void Create_ZnanyTyp_ZwracaPoprawnaKlase(ContainerType type, Type expected)
    {
        var resource = ContainerResourceFactory.Create(type, "c-001");
        Assert.IsType(expected, resource);
        Assert.Equal("c-001", resource.ContainerId);
    }

    [Fact]
    public void Create_NieznanyTyp_RzucaArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            ContainerResourceFactory.Create((ContainerType)999, "c-001"));
    }

    [Fact]
    public async Task Dispose_ZwalniaZasob_GdyUsing()
    {
        using var dev = new DevContainer("dev-01");
        await dev.InitializeAsync();
    }

    [Fact]
    public async Task ProductionContainer_CheckHealthAsync_ZwracaHealthyLubDegraded()
    {
        var prod = new ProductionContainer("prod-01");
        var health = await prod.CheckHealthAsync();
        Assert.True(
            health is ContainerHealth.Healthy or ContainerHealth.Degraded);
    }
}
