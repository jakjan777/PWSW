using ExpiringCacheDemo;
using Xunit;

namespace ExpiringCacheDemo.Tests;

public class ExpiringCacheTests
{
    [Fact]
    public void Set_And_Get_ReturnsValue()
    {
        var cache = new ExpiringCache<string, int>(TimeSpan.FromMinutes(5));
        cache.Set("key", 42);
        Assert.Equal(42, cache.Get("key"));
    }

    [Fact]
    public void Get_ExpiredItem_ReturnsDefault()
    {
        var cache = new ExpiringCache<string, int>(TimeSpan.FromMilliseconds(1));
        cache.Set("key", 42);
        Thread.Sleep(10);
        Assert.Equal(0, cache.Get("key"));
    }

    [Fact]
    public void Get_NonExistentKey_ReturnsDefault()
    {
        var cache = new ExpiringCache<string, string>(TimeSpan.FromMinutes(5));
        Assert.Null(cache.Get("missing"));
    }
}
