using Microsoft.EntityFrameworkCore;
using Warehouse.Domain;

namespace Warehouse.Infrastructure;

public class WriteDbContext(DbContextOptions<WriteDbContext> options) : DbContext(options)
{
    public DbSet<WarehouseProduct> Products => Set<WarehouseProduct>();
    public DbSet<AuditEntry> AuditLog => Set<AuditEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WarehouseProduct>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Sku).HasMaxLength(50);
            e.Property(p => p.Name).HasMaxLength(200);
            e.Property(p => p.Category).HasMaxLength(100);
            e.Ignore(p => p.DomainEvents);
        });

        modelBuilder.Entity<AuditEntry>(e =>
        {
            e.ToTable("AuditLog");
            e.HasKey(a => a.Id);
            e.Property(a => a.OperationId).HasMaxLength(64);
            e.Property(a => a.UserId).HasMaxLength(100);
            e.Property(a => a.Action).HasMaxLength(100);
            e.Property(a => a.Details).HasMaxLength(2000);
            e.Property(a => a.IpAddress).HasMaxLength(45);
        });
    }
}
