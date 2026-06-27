using FluentAssertions;
using Lab09_PackagedApp.Core.Models;

namespace Lab09_PackagedApp.Tests;

public class AppTests
{
    [Fact]
    public void AppVersion_ShouldBeValid()
    {
        var version = new Version("1.0.0.0");
        version.Major.Should().BeGreaterThanOrEqualTo(1);
    }

    [Theory]
    [InlineData("Hello", "HELLO")]
    [InlineData("world", "WORLD")]
    public void StringProcessor_ShouldUpperCase(string input, string expected)
    {
        input.ToUpperInvariant().Should().Be(expected);
    }

    [Fact]
    public void TaskItem_ShouldInitializeCorrectly()
    {
        var task = new TaskItem { Title = "Test" };
        task.Id.Should().NotBeEmpty();
        task.Title.Should().Be("Test");
        task.IsCompleted.Should().BeFalse();
        task.CreatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(5));
    }
}
