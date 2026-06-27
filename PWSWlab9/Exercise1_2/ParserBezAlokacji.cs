namespace Exercise1_2;

public static class ParserBezAlokacji
{
    public static bool CzyPodejrzanaLinia(
        ReadOnlySpan<char> linia, int progZnakowSpecjalnych = 3)
    {
        int znakiSpec = AnalizatorLogow.ZliczZnakiSpecjalne(linia);
        return znakiSpec >= progZnakowSpecjalnych;
    }

    public static int PoliczPoPoziomie(
        ReadOnlySpan<char> logi, ReadOnlySpan<char> poziom)
    {
        int licznik = 0;
        while (!logi.IsEmpty)
        {
            int koniec = logi.IndexOf('\n');
            var linia = koniec >= 0 ? logi[..koniec] : logi;
            if (linia.Contains(poziom, StringComparison.OrdinalIgnoreCase))
                licznik++;
            if (koniec < 0) break;
            logi = logi[(koniec + 1)..];
        }
        return licznik;
    }
}
