namespace IteratoryKolekcje;

public static class Sekwencje
{
    public static IEnumerable<long> Fibonacci()
    {
        long a = 0, b = 1;
        while (true)
        {
            yield return a;
            (a, b) = (b, a + b);
        }
    }

    public static IEnumerable<int> Zakres(int start, int koniec, int krok = 1)
    {
        for (int i = start; i < koniec; i += krok)
            yield return i;
    }

    public static IEnumerable<int> Collatz(int n)
    {
        while (true)
        {
            yield return n;
            if (n == 1) yield break;
            n = n % 2 == 0 ? n / 2 : 3 * n + 1;
        }
    }
}
