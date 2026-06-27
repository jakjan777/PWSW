using Exercise4_1.Documents;

namespace Exercise4_1.Creators;

public abstract class DocumentCreator
{
    protected abstract LegalDocument CreateDocument(string clientName);

    public LegalDocument PrepareDocument(string clientName)
    {
        Console.WriteLine("Przygotowywanie dokumentu...");
        var doc = CreateDocument(clientName);
        ValidateDocument(doc);
        Console.WriteLine($"Utworzono: {doc}");
        return doc;
    }

    protected virtual void ValidateDocument(LegalDocument doc)
    {
        if (string.IsNullOrWhiteSpace(doc.ClientName))
            throw new InvalidOperationException("Nazwa klienta jest wymagana.");
    }
}

public class ContractCreator(
    string counterparty, decimal value, DateTime validUntil)
    : DocumentCreator
{
    protected override LegalDocument CreateDocument(string clientName) =>
        new ContractDocument
        {
            Title = $"Umowa dla {clientName}",
            ClientName = clientName,
            CounterpartyName = counterparty,
            ContractValue = value,
            ValidUntil = validUntil
        };
}

public class PaymentDemandCreator(
    string debtor, decimal amount, int paymentDays)
    : DocumentCreator
{
    protected override LegalDocument CreateDocument(string clientName) =>
        new PaymentDemandDocument
        {
            Title = $"Wezwanie -- {clientName}",
            ClientName = clientName,
            DebtorName = debtor,
            Amount = amount,
            PaymentDays = paymentDays
        };
}
