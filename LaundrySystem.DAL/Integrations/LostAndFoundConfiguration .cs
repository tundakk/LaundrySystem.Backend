using LaundrySystem.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaundrySystem.DAL.Configurations
{
    public class LostAndFoundConfiguration : IEntityTypeConfiguration<LostAndFound>
    {
        public void Configure(EntityTypeBuilder<LostAndFound> builder)
        {
            builder.HasKey(lf => lf.Id);

            // Additional property configurations if needed

            // Seed data if necessary
            // builder.HasData( ... );
        }
    }
}