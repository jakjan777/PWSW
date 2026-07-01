using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace MultiArchSIMD;

public static class VectorOps
{
    public static float[] Add(float[] a, float[] b)
    {
        if (a.Length != b.Length)
            throw new ArgumentException("Tablice musza miec taka sama dlugosc");

        var result = new float[a.Length];

        if (Sse.IsSupported)
            AddSse(a, b, result);
        else if (AdvSimd.IsSupported)
            AddNeon(a, b, result);
        else
            AddScalar(a, b, result);

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
            {
                var va = Sse.LoadVector128(pa + i);
                var vb = Sse.LoadVector128(pb + i);
                Sse.Store(pr + i, Sse.Add(va, vb));
            }

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
            {
                var va = AdvSimd.LoadVector128(pa + i);
                var vb = AdvSimd.LoadVector128(pb + i);
                AdvSimd.Store(pr + i, AdvSimd.Add(va, vb));
            }

            for (; i < a.Length; i++)
                pr[i] = pa[i] + pb[i];
        }
    }

    private static void AddScalar(float[] a, float[] b, float[] result)
    {
        for (int i = 0; i < a.Length; i++)
            result[i] = a[i] + b[i];
    }
}

public static class PrismDetector
{
    public static bool IsEmulated() =>
        RuntimeInformation.ProcessArchitecture == Architecture.X64 &&
        RuntimeInformation.OSArchitecture == Architecture.Arm64;

    public static void PrintInfo()
    {
        Console.WriteLine($"Proces: {RuntimeInformation.ProcessArchitecture}");
        Console.WriteLine($"OS: {RuntimeInformation.OSArchitecture}");
        Console.WriteLine($"Prism: {IsEmulated()}");
        Console.WriteLine($"SSE: {Sse.IsSupported}");
        Console.WriteLine($"NEON: {AdvSimd.IsSupported}");
        Console.WriteLine($"Aktywna sciezka SIMD: {VectorOps.GetActivePath()}");
    }
}
