using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportOps.Domain.Entities;

namespace SupportOps.Infrastructure.Persistence.Configurations;

public class ContactRequestConfiguration
    : IEntityTypeConfiguration<ContactRequest>
{
    public void Configure(EntityTypeBuilder<ContactRequest> builder)
    {
        builder.ToTable("ContactRequests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FullName)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.Company)
               .HasMaxLength(100);

        builder.Property(x => x.Email)
               .HasMaxLength(150)
               .IsRequired();

        builder.Property(x => x.Phone)
               .HasMaxLength(30);

        builder.Property(x => x.Service)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(x => x.Message)
               .HasMaxLength(2000)
               .IsRequired();
    }
}