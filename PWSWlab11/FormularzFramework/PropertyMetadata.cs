using System.Reflection;

namespace FormularzFramework;

public class PropertyMetadata
{
    public PropertyInfo Property { get; init; } = null!;
    public string DisplayName { get; init; } = "";
    public bool IsRequired { get; init; }
    public RangeAttribute? Range { get; init; }
    public MaxLengthAttribute? MaxLength { get; init; }
    public bool IsEmail { get; init; }
    public int Order { get; init; }
}
