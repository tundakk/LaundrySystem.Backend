using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LaundrySystem.DAL.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            // Identity configuration
            builder.HasKey(u => u.Id);

            // Additional property configurations if needed

            //// Seed data
            //var passwordHasher = new PasswordHasher<AppUser>();

            //var user1 = new AppUser
            //{
            //    Id = Guid.Parse("115b5117-73f6-4796-a87a-962181baa3e5"),
            //    UserName = "user1@example.com",
            //    NormalizedUserName = "USER1@EXAMPLE.COM",
            //    Email = "user1@example.com",
            //    NormalizedEmail = "USER1@EXAMPLE.COM",
            //    EmailConfirmed = true,
            //    SecurityStamp = Guid.NewGuid().ToString("D"),
            //    ApartmentNumber = 101,
            //    PhoneNumberSecondary = "555-12345",
            //    EmailOptOut = false,
            //    SmsOptOut = true,
            //    PinCode = 1234
            //};
            //user1.PasswordHash = passwordHasher.HashPassword(user1, "Password123!");

            //var user2 = new AppUser
            //{
            //    Id = Guid.Parse("225b5117-73f6-4796-a87a-962181baa3e5"),
            //    UserName = "user2@example.com",
            //    NormalizedUserName = "USER2@EXAMPLE.COM",
            //    Email = "user2@example.com",
            //    NormalizedEmail = "USER2@EXAMPLE.COM",
            //    EmailConfirmed = true,
            //    SecurityStamp = Guid.NewGuid().ToString("D"),
            //    ApartmentNumber = 102,
            //    EmailOptOut = true,
            //    SmsOptOut = false,
            //    PinCode = 5678
            //};
            //user2.PasswordHash = passwordHasher.HashPassword(user2, "Password123!");

            //var user3 = new AppUser
            //{
            //    Id = Guid.Parse("335b5117-73f6-4796-a87a-962181baa3e5"),
            //    UserName = "user3@example.com",
            //    NormalizedUserName = "USER3@EXAMPLE.COM",
            //    Email = "user3@example.com",
            //    NormalizedEmail = "USER3@EXAMPLE.COM",
            //    EmailConfirmed = true,
            //    SecurityStamp = Guid.NewGuid().ToString("D"),
            //    ApartmentNumber = 103,
            //    PhoneNumberSecondary = "555-67890",
            //    EmailOptOut = false,
            //    SmsOptOut = true,
            //    PinCode = 9012
            //};
            //user3.PasswordHash = passwordHasher.HashPassword(user3, "Password123!");

            //builder.HasData(user1, user2, user3);
        }
    }
}