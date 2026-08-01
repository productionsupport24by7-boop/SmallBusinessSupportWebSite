using Microsoft.EntityFrameworkCore;
using SupportOps.Domain.Entities;



namespace SupportOps.Infrastructure.Data;

public class SupportOpsDbContext : DbContext
{
    public SupportOpsDbContext(DbContextOptions<SupportOpsDbContext> options)
        : base(options)
    {
    }

    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<ContactRequest> ContactRequests => Set<ContactRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SupportOpsDbContext).Assembly);
    }


}