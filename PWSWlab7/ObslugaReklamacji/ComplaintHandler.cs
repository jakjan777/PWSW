namespace ObslugaReklamacji;

public abstract class ComplaintHandler
{
    private ComplaintHandler? _next;

    public ComplaintHandler SetNext(ComplaintHandler next)
    {
        _next = next;
        return next;
    }

    public virtual void Handle(Complaint complaint)
    {
        if (_next is not null)
            _next.Handle(complaint);
        else
            complaint.ProcessingLog.Add("Koniec lancucha -- brak dalszych handlerow.");
    }
}

public class WarrantyCheckHandler : ComplaintHandler
{
    private const int WarrantyMonths = 24;

    public override void Handle(Complaint complaint)
    {
        var mies = (DateOnly.FromDateTime(DateTime.Today).DayNumber
                    - complaint.PurchaseDate.DayNumber) / 30;
        if (mies > WarrantyMonths)
        {
            complaint.Status = ComplaintStatus.Rejected;
            complaint.ProcessingLog.Add(
                $"[Gwarancja] ODRZUCONO -- {mies}/{WarrantyMonths} mies.");
            return;
        }

        complaint.Status = ComplaintStatus.WarrantyVerified;
        complaint.ProcessingLog.Add($"[Gwarancja] OK -- {mies}/{WarrantyMonths} mies.");
        base.Handle(complaint);
    }
}

public class DamageAssessmentHandler : ComplaintHandler
{
    public override void Handle(Complaint complaint)
    {
        if (complaint.Damage == DamageType.CustomerFault)
        {
            complaint.Status = ComplaintStatus.Rejected;
            complaint.ProcessingLog.Add("[Ocena szkody] ODRZUCONO -- wina klienta.");
            return;
        }

        complaint.ProcessingLog.Add("[Ocena szkody] OK -- szkoda objeta reklamacja.");
        base.Handle(complaint);
    }
}

public class RefundApprovalHandler : ComplaintHandler
{
    private const decimal LimitNadzoru = 2000m;

    public override void Handle(Complaint complaint)
    {
        complaint.Status = ComplaintStatus.Approved;
        if (complaint.ProductPrice > LimitNadzoru)
        {
            complaint.ProcessingLog.Add(
                $"[Zwrot] ZATWIERDZONO -- kwota {complaint.ProductPrice:F2} zl przekracza limit {LimitNadzoru:F2} zl, wymaga nadzoru.");
        }
        else
        {
            complaint.ProcessingLog.Add(
                $"[Zwrot] ZATWIERDZONO -- kwota {complaint.ProductPrice:F2} zl.");
        }

        base.Handle(complaint);
    }
}
