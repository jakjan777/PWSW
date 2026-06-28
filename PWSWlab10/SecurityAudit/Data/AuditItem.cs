namespace SecurityAudit.Data;

public class AuditItem
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public int Priority { get; set; }
}
