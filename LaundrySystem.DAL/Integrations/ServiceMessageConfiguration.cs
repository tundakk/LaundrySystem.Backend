using LaundrySystem.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaundrySystem.DAL.Configurations;

public class ServiceMessageConfiguration : IEntityTypeConfiguration<ServiceMessage>
{
    public void Configure(EntityTypeBuilder<ServiceMessage> builder)
    {
        builder.HasKey(sm => sm.Id);

        builder.Property(sm => sm.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(sm => sm.Body)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(sm => sm.Severity)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(sm => sm.ActiveFrom)
            .IsRequired();

        builder.Ignore(sm => sm.IsActive);

        // Multi-tenancy: ServiceMessage belongs to an account
        builder.HasOne(sm => sm.Account)
            .WithMany()
            .HasForeignKey(sm => sm.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        // Optional: Can be scoped to a specific building
        builder.HasOne(sm => sm.Building)
            .WithMany()
            .HasForeignKey(sm => sm.BuildingId)
            .OnDelete(DeleteBehavior.SetNull);

        // Index for faster queries within an account
        builder.HasIndex(sm => sm.AccountId);
        builder.HasIndex(sm => sm.BuildingId);
    }
}
