using DateParserDemo;
using Xunit;

namespace DateParserDemo.Tests;

// Test z blednym zalozeniem -- PADNIE przed naprawa:
// rok 2026 nie jest przestepny, DateTime.TryParse zwraca false, Parse zwraca null
public class DateParserBrokenTests
{
    [Fact]
    public void Parser_ShouldHandleEdgeCases()
    {
        var parser = new DateParser();
        var result = parser.Parse("2028-02-29");
        Assert.NotNull(result);
    }
}
