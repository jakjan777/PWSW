namespace DiagnosticsApi.Contracts;

public record BenchmarkResponse(double ElapsedMs, string SIMDPath);

public record ArchInfoResponse(
    string ProcessArch,
    string OsArch,
    bool IsEmulated,
    bool HasSse,
    bool HasNeon,
    string? Warning,
    BenchmarkResponse Benchmark);

public static class ArchInfoMapper
{
    public static ArchInfoResponse ToResponse(ArchInfo info) =>
        new(
            info.ProcessArch.ToString(),
            info.OsArch.ToString(),
            info.IsEmulated,
            info.HasSse,
            info.HasNeon,
            info.IsEmulated
                ? "Aplikacja dziala przez emulacje Prism x64 na ARM64 co moze obnizyc wydajnosc obliczen"
                : null,
            new BenchmarkResponse(info.BenchmarkMs, info.SimdPath));
}
