using System.Text.RegularExpressions;

namespace Challenge2.Domain;

public record AppVersion(int Major, int Minor, int Build, int Revision)
{
    public static AppVersion Parse(string versionString)
    {
        var match = VersionParser.RegexVersion().Match(versionString);
        if (!match.Success)
            throw new FormatException($"Nieprawidlowy format wersji: '{versionString}'");

        var revision = match.Groups["revision"].Success
            ? int.Parse(match.Groups["revision"].Value)
            : 0;

        return new AppVersion(
            int.Parse(match.Groups["major"].Value),
            int.Parse(match.Groups["minor"].Value),
            int.Parse(match.Groups["build"].Value),
            revision);
    }

    public AppVersion BumpBuild() =>
        this with { Build = Build + 1, Revision = 0 };

    public override string ToString() =>
        $"{Major}.{Minor}.{Build}.{Revision}";
}
