using System.Reflection;

namespace FormularzFramework;

public static class MetadataReader
{
    public static List<PropertyMetadata> CzytajMetadane<T>()
    {
        Type typ = typeof(T);
        var wynik = new List<PropertyMetadata>();

        foreach (PropertyInfo prop in typ.GetProperties())
        {
            var displayName = prop.GetCustomAttribute<DisplayNameAttribute>();
            var order = prop.GetCustomAttribute<DisplayOrderAttribute>();

            wynik.Add(new PropertyMetadata
            {
                Property = prop,
                DisplayName = displayName?.Name ?? prop.Name,
                IsRequired = prop.GetCustomAttribute<RequiredAttribute>() is not null,
                Range = prop.GetCustomAttribute<RangeAttribute>(),
                MaxLength = prop.GetCustomAttribute<MaxLengthAttribute>(),
                IsEmail = prop.GetCustomAttribute<EmailAddressAttribute>() is not null,
                Order = order?.Order ?? 999
            });
        }

        return wynik.OrderBy(m => m.Order).ToList();
    }
}
