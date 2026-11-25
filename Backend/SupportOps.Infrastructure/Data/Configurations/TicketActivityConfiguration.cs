using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportOps.Domain.Entities;

namespace SupportOps.Infrastructure.Data.Configurations;

public class TicketActivityConfiguration
    : IEntityTypeConfiguration<TicketActivity>
{
    public void Configure(
        EntityTypeBuilder<TicketActivity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ActivityType)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.FieldName)
            .HasMaxLength(100);

        builder.Property(x => x.OldValue)
            .HasMaxLength(2000);

        builder.Property(x => x.NewValue)
            .HasMaxLength(2000);

        builder.Property(x => x.Comment)
            .HasMaxLength(4000);

        builder.Property(x => x.ChangedByUserId)
            .IsRequired();

        builder.Property(x => x.CreatedOn)
            .IsRequired();

        builder.HasOne(x => x.Ticket)
            .WithMany(x => x.Activities)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new
        {
            x.TicketId,
            x.CreatedOn
        });
    }
}