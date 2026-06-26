namespace DisposePatterns.Models;

public record SaleRecord(
    DateOnly Date,
    string Branch,
    string Product,
    int Quantity,
    decimal Total);
