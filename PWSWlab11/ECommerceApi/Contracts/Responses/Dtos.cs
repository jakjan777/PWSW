namespace ECommerceApi.Contracts.Responses;

public record ProductResponse(
    Guid Id, string Name, string Description,
    decimal Price, bool Available, string Sku);

public record CustomerResponse(
    Guid Id, string FullName, string Email,
    string PhoneNumber, bool IsActive);

public record OrderItemResponse(
    Guid ProductId, string ProductName,
    int Quantity, decimal UnitPrice, decimal Subtotal);

public record OrderResponse(
    Guid Id, CustomerResponse Customer,
    DateTime OrderDate, string Status,
    List<OrderItemResponse> Items, decimal TotalAmount, int TotalItems);

public record PagedResponse<T>(List<T> Items, int TotalCount, int Page, int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => Page < TotalPages;
}
