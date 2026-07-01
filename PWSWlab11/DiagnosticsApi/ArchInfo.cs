using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace DiagnosticsApi;

public static class VectorOps
{
    public static float[] Add(float[] a, float[] b)
    {
        var result = new float[a.Length];
        if (Sse.IsSupported)
            AddSse(a, b, result);
        else if (AdvSimd.IsSupported)
            AddNeon(a, b, result);
        else
            for (int i = 0; i < a.Length; i++)
                result[i] = a[i] + b[i];
        return result;
    }

    public static string GetActivePath()
    {
        if (Sse.IsSupported) return "SSE";
        if (AdvSimd.IsSupported) return "NEON";
        return "Scalar";
    }

    private static unsafe void AddSse(float[] a, float[] b, float[] result)
    {
        fixed (float* pa = a, pb = b, pr = result)
        {
            int i = 0;
            for (; i <= a.Length - 4; i += 4)
                Sse.Store(pr + i, Sse.Add(Sse.LoadVector128(pa + i), Sse.LoadVector128(pb + i)));
            for (; i < a.Length; i++)
                pr[i] = pa[i] + pb[i];
        }
    }

    private static unsafe void AddNeon(float[] a, float[] b, float[] result)
    {
        fixed (float* pa = a, pb = b, pr = result)
        {
            int i = 0;
            for (; i <= a.Length - 4; i += 4)
                AdvSimd.Store(pr + i, AdvSimd.Add(AdvSimd.LoadVector128(pa + i), AdvSimd.LoadVector128(pb + i)));
            for (; i < a.Length; i++)
                pr[i] = pa[i] + pb[i];
        }
    }
}

public class ArchInfo
{
    public Architecture ProcessArch { get; init; }
    public Architecture OsArch { get; init; }
    public bool IsEmulated { get; init; }
    public bool HasSse { get; init; }
    public bool HasNeon { get; init; }
    public double BenchmarkMs { get; init; }
    public string SimdPath { get; init; } = "";

    public static ArchInfo Collect()
    {
        const int size = 100_000;
        var a = Enumerable.Range(0, size).Select(i => (float)i).ToArray();
        var b = Enumerable.Range(0, size).Select(i => (float)(i * 2)).ToArray();

        VectorOps.Add(a, b);

        var sw = Stopwatch.StartNew();
        VectorOps.Add(a, b);
        sw.Stop();

        return new ArchInfo
        {
            ProcessArch = RuntimeInformation.ProcessArchitecture,
            OsArch = RuntimeInformation.OSArchitecture,
            IsEmulated = RuntimeInformation.ProcessArchitecture == Architecture.X64
                && RuntimeInformation.OSArchitecture == Architecture.Arm64,
            HasSse = Sse.IsSupported,
            HasNeon = AdvSimd.IsSupported,
            BenchmarkMs = sw.Elapsed.TotalMilliseconds,
            SimdPath = VectorOps.GetActivePath()
        };
    }
}
