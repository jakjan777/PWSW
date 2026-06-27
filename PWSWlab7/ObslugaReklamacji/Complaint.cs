namespace ObslugaReklamacji;

public enum ComplaintStatus
{
    Pending,
    Rejected,
    WarrantyVerified,
    Approved
}

public enum DamageType
{
    CriticalDefect,
    MinorDefect,
    CustomerFault
}

public class Complaint
{
    public required string ComplaintId { get; init; }
    public required string ProductName { get; init; }
    public required DateOnly PurchaseDate { get; init; }
    public required DamageType Damage { get; init; }
    public required decimal ProductPrice { get; init; }
    public ComplaintStatus Status { get; set; } = ComplaintStatus.Pending;
    public List<string> ProcessingLog { get; } = [];
}
