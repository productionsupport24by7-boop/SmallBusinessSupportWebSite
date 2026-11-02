using Microsoft.EntityFrameworkCore;
using SupportOps.Domain.Entities;
using SupportOps.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;



namespace SupportOps.Infrastructure.Data;

// public class SupportOpsDbContext : DbContext
public class SupportOpsDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public SupportOpsDbContext(DbContextOptions<SupportOpsDbContext> options)
        : base(options)
    {
    }

    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<ContactRequest> ContactRequests => Set<ContactRequest>();
    // public DbSet<User> Users => Set<User>();
    public DbSet<Ticket> Tickets { get; set; }

    public DbSet<EmailTemplate> EmailTemplates { get; set; }

    public DbSet<EmailOutbox> EmailOutbox { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SupportOpsDbContext).Assembly);
        modelBuilder.Entity<EmailTemplate>(entity =>
        {
            entity.HasIndex(e => e.TemplateCode).IsUnique();

            entity.Property(e => e.TemplateCode)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.Subject)
                .HasMaxLength(250)
                .IsRequired();
        });

        modelBuilder.Entity<EmailOutbox>(entity =>
        {
            entity.Property(e => e.ToEmail)
                .HasMaxLength(250)
                .IsRequired();

            entity.HasIndex(e => new { e.Status, e.CreatedOn });
        });
    }


}