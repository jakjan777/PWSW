using System.Data;
using Dapper;
using MediatR;

namespace Warehouse.Application.Queries;

public class ProductListItem
{
    public string Id { get; set; } = "";
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public int QuantityOnHand { get; set; }
    public string StockStatus { get; set; } = "";
}

public record GetProductsQuery(string? Category)
    : IRequest<List<ProductListItem>>;

public class GetProductsHandler(IDbConnection readDb)
    : IRequestHandler<GetProductsQuery, List<ProductListItem>>
{
    public async Task<List<ProductListItem>> Handle(
        GetProductsQuery query, CancellationToken ct)
    {
        const string sql = """
            SELECT Id, Sku, Name, QuantityOnHand,
            CASE
                WHEN QuantityOnHand = 0 THEN 'Brak'
                WHEN QuantityOnHand <= ReorderLevel THEN 'Niski stan'
                ELSE 'OK'
            END AS StockStatus
            FROM Products
            WHERE (@Category IS NULL OR Category = @Category)
            ORDER BY Name
            """;

        var result = await readDb.QueryAsync<ProductListItem>(
            sql, new { query.Category });
        return result.ToList();
    }
}
