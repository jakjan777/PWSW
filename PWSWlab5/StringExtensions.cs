namespace PWSW5;

public static class StringExtensions
{
    public static bool JestPoprawnymIBAN(this string iban)
    {
        string oczyszczony = iban.Replace(" ", "").ToUpperInvariant();
        if (oczyszczony.Length < 5 || oczyszczony.Length > 34)
            return false;
        if (!char.IsLetter(oczyszczony[0]) || !char.IsLetter(oczyszczony[1]))
            return false;
        if (!char.IsDigit(oczyszczony[2]) || !char.IsDigit(oczyszczony[3]))
            return false;
        return oczyszczony[4..].All(char.IsLetterOrDigit);
    }

    public static string FormatujIBAN(this string iban)
    {
        string oczyszczony = iban.Replace(" ", "").ToUpperInvariant();
        return string.Join(" ",
            Enumerable.Range(0, (oczyszczony.Length + 3) / 4)
                .Select(i => oczyszczony.Substring(
                    i * 4, Math.Min(4, oczyszczony.Length - i * 4))));
    }

    public static string Zamaskuj(this string tekst, int widoczneNaPoczatku = 4)
    {
        if (tekst.Length <= widoczneNaPoczatku) return tekst;
        return tekst[..widoczneNaPoczatku] +
               new string('*', tekst.Length - widoczneNaPoczatku);
    }
}
