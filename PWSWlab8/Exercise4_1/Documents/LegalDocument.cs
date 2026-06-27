namespace Exercise4_1.Documents;

public enum DocumentType
{
    Contract,
    PowerOfAttorney,
    Lawsuit,
    PaymentDemand
}

public abstract class LegalDocument
{
    public string DocumentId { get; init; } =
        Guid.NewGuid().ToString("N")[..8].ToUpper();

    public DateTime CreatedAt { get; init; } = DateTime.Now;
    public required string Title { get; init; }
    public required string ClientName { get; init; }
    public abstract DocumentType Type { get; }
    public abstract string GenerateContent();

    public override string ToString() =>
        $"[{Type}] {DocumentId} -- {Title} (klient: {ClientName})";
}

public class ContractDocument : LegalDocument
{
    public override DocumentType Type => DocumentType.Contract;
    public required string CounterpartyName { get; init; }
    public required decimal ContractValue { get; init; }
    public required DateTime ValidUntil { get; init; }

    public override string GenerateContent() =>
        $"UMOWA nr {DocumentId}\nStrony: {ClientName} / {CounterpartyName}\n" +
        $"Wartosc: {ContractValue:C}, wazna do: {ValidUntil:d}";
}

public class PaymentDemandDocument : LegalDocument
{
    public override DocumentType Type => DocumentType.PaymentDemand;
    public required string DebtorName { get; init; }
    public required decimal Amount { get; init; }
    public required int PaymentDays { get; init; }

    public override string GenerateContent() =>
        $"WEZWANIE DO ZAPLATY nr {DocumentId}\n" +
        $"Wierzyciel: {ClientName}, Dluznik: {DebtorName}\n" +
        $"Kwota: {Amount:C}, termin: {PaymentDays} dni";
}
