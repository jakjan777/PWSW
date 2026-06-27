using System.Text.RegularExpressions;

namespace Challenge2.Domain;

public partial class VersionParser
{
    [GeneratedRegex(
        @"(?<major>\d+)\.(?<minor>\d+)\.(?<build>\d+)(?:\.(?<revision>\d+))?",
        RegexOptions.Compiled)]
    public static partial Regex RegexVersion();
}
