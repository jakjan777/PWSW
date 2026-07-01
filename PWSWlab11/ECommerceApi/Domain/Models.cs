namespace ECommerceApi.Domain;

public enum OrderStatus { Pending, Confirmed, Shipped, Delivered, Cancelled }

public class Product
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal CostPrice { get; set; }
    public decimal Margin { get; set; }
    public decimal SellingPrice => CostPrice + Margin;
    public int StockQuantity { get; set; }
    public bool IsInStock => StockQuantity > 0;
    public string Sku { get; set; } = "";
}

public class Customer
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public bool IsActive { get; set; } = true;
}

public class OrderItem
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Product Product { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Subtotal => Quantity * UnitPrice;
}

public class Order
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Customer Customer { get; set; } = null!;
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public List<OrderItem> Items { get; set; } = [];
    public decimal TotalAmount => Items.Sum(i => i.Subtotal);
    public int TotalItems => Items.Sum(i => i.Quantity);
}
