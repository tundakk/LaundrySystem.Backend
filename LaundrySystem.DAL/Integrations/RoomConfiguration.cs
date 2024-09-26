using LaundrySystem.Domain.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaundrySystem.DAL.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasKey(r => r.Id);

            // Additional property configurations if needed

            // Seed data
            var room1Id = Guid.Parse("0c9c62e6-2d71-4a7d-b38e-fd3d9f8c0a6f");
            var room2Id = Guid.Parse("1d8e57f9-5e9b-4c4b-9f2d-1c4e7a1e2b2c");

            //builder.HasData(
            //    new Room
            //    {
            //        Id = room1Id,
            //        Name = "Room A",
            //        Location = "Building 1",
            //        IsAvailable = true,
            //        MaxCapacity = 10
            //    },
            //    new Room
            //    {
            //        Id = room2Id,
            //        Name = "Room B",
            //        Location = "Building 2",
            //        IsAvailable = false,
            //        MaxCapacity = 8
            //    }
            //);
        }
    }
}