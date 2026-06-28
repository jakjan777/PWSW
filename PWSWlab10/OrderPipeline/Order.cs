namespace OrderPipeline;

public enum OrderStatus { New, Validated, Paid, Shipped, Error }

public class Order
{
    public string Id { get; set; } = string.Empty;
    public string Client { get; set; } = string.Empty;
    public string Product { get; set; } = string.Empty;
    public int Qty { get; set; }
    public decimal Price { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.New;
    public string? ErrorMsg { get; set; }
}
