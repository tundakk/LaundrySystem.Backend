using LaundrySystem.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaundrySystem.DAL.Configurations
{
    public class TimeslotConfiguration : IEntityTypeConfiguration<Timeslot>
    {
        public void Configure(EntityTypeBuilder<Timeslot> builder)
        {
            builder.HasKey(t => t.Id);

            // Configure the owned type (TimeRange)
            builder.OwnsOne(t => t.SlotTime, sa =>
            {
                sa.Property(p => p.Start).HasColumnName("StartTime");
                sa.Property(p => p.End).HasColumnName("EndTime");
            });
        }
    }
}