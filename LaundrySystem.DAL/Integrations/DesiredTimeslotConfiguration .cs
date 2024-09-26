using LaundrySystem.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaundrySystem.DAL.Configurations
{
    public class DesiredTimeslotConfiguration : IEntityTypeConfiguration<DesiredTimeslot>
    {
        public void Configure(EntityTypeBuilder<DesiredTimeslot> builder)
        {
            builder.HasKey(dt => dt.Id);

            builder.HasOne(dt => dt.User)
                .WithMany(u => u.DesiredTimeslots)
                .HasForeignKey(dt => dt.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(dt => dt.Timeslot)
                .WithMany(t => t.DesiredTimeslots)
                .HasForeignKey(dt => dt.TimeslotId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data if necessary
            // builder.HasData( ... );
        }
    }
}