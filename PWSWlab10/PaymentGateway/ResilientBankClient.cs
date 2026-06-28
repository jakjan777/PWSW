namespace PaymentGateway;

public class ResilientBankClient(HttpClient http)
{
    public Task<HttpResponseMessage> GetBalanceAsync(CancellationToken ct = default) =>
        http.GetAsync("/balance", ct);
}
