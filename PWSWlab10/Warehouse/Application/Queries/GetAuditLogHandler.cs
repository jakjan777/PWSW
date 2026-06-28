using System.Data;
using Dapper;
using MediatR;

namespace Warehouse.Application.Queries;

public class AuditLogItem
{
    public string Id { get; set; } = "";
    public string OperationId { get; set; } = "";
    public string UserId { get; set; } = "";
    public string Action { get; set; } = "";
    public string Details { get; set; } = "";
    public string? IpAddress { get; set; }
    public DateTime OccurredAt { get; set; }
}

public record GetAuditLogQuery(int Page = 1, int PageSize = 10)
    : IRequest<List<AuditLogItem>>;

public class GetAuditLogHandler(IDbConnection readDb)
    : IRequestHandler<GetAuditLogQuery, List<AuditLogItem>>
{
    public async Task<List<AuditLogItem>> Handle(
        GetAuditLogQuery query, CancellationToken ct)
    {
        const string sql = """
            SELECT Id, OperationId, UserId, Action, Details, IpAddress, OccurredAt
            FROM AuditLog
            ORDER BY OccurredAt DESC
            LIMIT @PageSize OFFSET @Offset
            """;

        var result = await readDb.QueryAsync<AuditLogItem>(sql, new
        {
            query.PageSize,
            Offset = (query.Page - 1) * query.PageSize
        });
        return result.ToList();
    }
}
