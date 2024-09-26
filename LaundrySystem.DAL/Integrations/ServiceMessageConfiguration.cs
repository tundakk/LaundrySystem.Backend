using LaundrySystem.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaundrySystem.DAL.Configurations
{
    public class ServiceMessageConfiguration : IEntityTypeConfiguration<ServiceMessage>
    {
        public void Configure(EntityTypeBuilder<ServiceMessage> builder)
        {
            builder.HasKey(sm => sm.Id);

            // Additional property configurations if needed

            //// Seed data
            //builder.HasData(
            //    new ServiceMessage
            //    {
            //        Id = Guid.NewGuid(),
            //        Message = "Welcome to LaundrySystem!",
            //        CreatedAt = DateTime.UtcNow
            //    },
            //    new ServiceMessage
            //    {
            //        Id = Guid.NewGuid(),
            //        Message = "Please adhere to the laundry schedule.",
            //        CreatedAt = DateTime.UtcNow
            //    },
            //    new ServiceMessage
            //    {
            //        Id = Guid.NewGuid(),
            //        Message = "Thank you for using LaundrySystem.",
            //        CreatedAt = DateTime.UtcNow
            //    }
            //);
        }
    }
}