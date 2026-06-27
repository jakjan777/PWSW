using ObslugaReklamacji;

var warranty = new WarrantyCheckHandler();
var damage = new DamageAssessmentHandler();
var refund = new RefundApprovalHandler();

warranty.SetNext(damage).SetNext(refund);

var reklamacje =
    new[]
    {
        new Complaint
        {
            ComplaintId = "REC-001",
            ProductName = "Laptop ProMax 15",
            PurchaseDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(-6)),
            Damage = DamageType.CriticalDefect,
            ProductPrice = 4599.99m
        },
        new Complaint
        {
            ComplaintId = "REC-002",
            ProductName = "Monitor LED 27",
            PurchaseDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(-30)),
            Damage = DamageType.MinorDefect,
            ProductPrice = 899.00m
        },
        new Complaint
        {
            ComplaintId = "REC-003",
            ProductName = "Smartfon X12",
            PurchaseDate = DateOnly.FromDateTime(DateTime.Today.AddMonths(-3)),
            Damage = DamageType.CustomerFault,
            ProductPrice = 2499.00m
        }
    };

foreach (var reklamacja in reklamacje)
{
    Console.WriteLine($"--- {reklamacja.ComplaintId}: {reklamacja.ProductName} ---");
    warranty.Handle(reklamacja);
    Console.WriteLine($"Status: {reklamacja.Status}");
    foreach (var wpis in reklamacja.ProcessingLog)
        Console.WriteLine($"  {wpis}");
    Console.WriteLine();
}
