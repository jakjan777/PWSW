namespace DateParserDemo;

public class DateParser
{
    public DateTime? Parse(string input) =>
        DateTime.TryParse(input, out var result) ? result : null;
}
