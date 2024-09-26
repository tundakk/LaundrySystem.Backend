// IdentityRoleConfiguration.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaundrySystem.DAL.Configurations
{
    public class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
        {
            //builder.HasData(
            //    new IdentityRole<Guid>
            //    {
            //        Id = Guid.NewGuid(), // Ensure this is unique
            //        Name = "Admin",
            //        NormalizedName = "ADMIN"
            //    },
            //    new IdentityRole<Guid>
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "User",
            //        NormalizedName = "USER"
            //    },
            //    new IdentityRole<Guid>
            //    {
            //        Id = Guid.NewGuid(),
            //        Name = "Manager",
            //        NormalizedName = "MANAGER"
            //    }
            //);
        }
    }
}