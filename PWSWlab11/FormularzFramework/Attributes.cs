namespace FormularzFramework;

[AttributeUsage(AttributeTargets.Property)]
public class DisplayNameAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}

[AttributeUsage(AttributeTargets.Property)]
public class RequiredAttribute : Attribute
{
    public string? ErrorMessage { get; set; }
}

[AttributeUsage(AttributeTargets.Property)]
public class RangeAttribute(double min, double max) : Attribute
{
    public double Min { get; } = min;
    public double Max { get; } = max;
    public string? ErrorMessage { get; set; }
}

[AttributeUsage(AttributeTargets.Property)]
public class MaxLengthAttribute(int length) : Attribute
{
    public int Length { get; } = length;
}

[AttributeUsage(AttributeTargets.Property)]
public class EmailAddressAttribute : Attribute;

[AttributeUsage(AttributeTargets.Property)]
public class DisplayOrderAttribute(int order) : Attribute
{
    public int Order { get; } = order;
}
