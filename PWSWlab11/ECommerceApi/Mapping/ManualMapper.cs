using ECommerceApi.Contracts.Responses;
using ECommerceApi.Domain;

namespace ECommerceApi.Mapping;

public static class ManualMapper
{
    public static ProductResponse ToResponse(this Product product) =>
        new(product.Id, product.Name, product.Description,
            product.SellingPrice,
            product.IsInStock,
            product.Sku);

    public static CustomerResponse ToResponse(this Customer customer) =>
        new(customer.Id, customer.FullName, customer.Email,
            customer.PhoneNumber, customer.IsActive);

    public static OrderItemResponse ToResponse(this OrderItem item) =>
        new(item.Product.Id, item.Product.Name,
            item.Quantity, item.UnitPrice, item.Subtotal);

    public static OrderResponse ToResponse(this Order order) =>
        new(order.Id,
            order.Customer.ToResponse(),
            order.OrderDate,
            order.Status.ToString(),
            order.Items.Select(i => i.ToResponse()).ToList(),
            order.TotalAmount,
            order.TotalItems);
}
