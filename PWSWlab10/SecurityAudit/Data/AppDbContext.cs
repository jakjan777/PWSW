using Microsoft.EntityFrameworkCore;

namespace SecurityAudit.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<AuditItem> Audits => Set<AuditItem>();
}
