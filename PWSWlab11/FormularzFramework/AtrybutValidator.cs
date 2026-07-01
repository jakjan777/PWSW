using System.Reflection;

namespace FormularzFramework;

public static class AtrybutValidator
{
    public static List<string> Waliduj(object obiekt)
    {
        var bledy = new List<string>();
        Type typ = obiekt.GetType();

        foreach (PropertyInfo prop in typ.GetProperties())
        {
            object? wartosc = prop.GetValue(obiekt);
            string nazwa = prop.GetCustomAttribute<DisplayNameAttribute>()?.Name
                ?? prop.Name;

            var required = prop.GetCustomAttribute<RequiredAttribute>();
            if (required is not null &&
                (wartosc is null || (wartosc is string s && string.IsNullOrWhiteSpace(s))))
            {
                bledy.Add(required.ErrorMessage ?? $"{nazwa} jest wymagane");
                continue;
            }

            var range = prop.GetCustomAttribute<RangeAttribute>();
            if (range is not null && wartosc is not null)
            {
                double liczba = Convert.ToDouble(wartosc);
                if (liczba < range.Min || liczba > range.Max)
                    bledy.Add(range.ErrorMessage
                        ?? $"{nazwa} musi byc miedzy {range.Min} a {range.Max}");
            }

            var maxLen = prop.GetCustomAttribute<MaxLengthAttribute>();
            if (maxLen is not null && wartosc is string tekst && tekst.Length > maxLen.Length)
                bledy.Add($"{nazwa}: max {maxLen.Length} znakow");

            if (prop.GetCustomAttribute<EmailAddressAttribute>() is not null
                && wartosc is string email
                && (!email.Contains('@') || !email.Contains('.')))
                bledy.Add($"{nazwa}: niepoprawny format email");
        }

        return bledy;
    }
}
