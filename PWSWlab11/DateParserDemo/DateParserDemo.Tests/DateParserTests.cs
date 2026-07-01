using DateParserDemo;
using Xunit;

namespace DateParserDemo.Tests;

public class DateParserTests
{
    [Fact]
    public void Parser_ShouldReturnNullForInvalidDate()
    {
        var parser = new DateParser();
        Assert.Null(parser.Parse("2026-02-29"));
    }

    [Fact]
    public void Parser_ShouldParseValidLeapYearDate()
    {
        var parser = new DateParser();
        var result = parser.Parse("2028-02-29");
        Assert.NotNull(result);
        Assert.Equal(29, result!.Value.Day);
    }
}
