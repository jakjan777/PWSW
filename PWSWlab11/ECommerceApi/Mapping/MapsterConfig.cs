using ECommerceApi.Contracts.Responses;
using ECommerceApi.Domain;
using Mapster;

namespace ECommerceApi.Mapping;

public static class MapsterConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<Product, ProductResponse>.NewConfig()
            .Map(dest => dest.Price, src => src.SellingPrice)
            .Map(dest => dest.Available, src => src.IsInStock);

        TypeAdapterConfig<OrderItem, OrderItemResponse>.NewConfig()
            .Map(dest => dest.ProductId, src => src.Product.Id)
            .Map(dest => dest.ProductName, src => src.Product.Name);

        TypeAdapterConfig<Order, OrderResponse>.NewConfig()
            .Map(dest => dest.Status, src => src.Status.ToString())
            .Map(dest => dest.Customer, src => src.Customer.Adapt<CustomerResponse>());

        TypeAdapterConfig.GlobalSettings.Compile();
    }
}
