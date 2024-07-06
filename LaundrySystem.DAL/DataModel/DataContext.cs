namespace LaundrySystem.DAL.DataModel
{
    using LaundrySystem.DAL.Entities;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class DataContext : IdentityDbContext<IdentityUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Household> Households { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<LaundryReservation> LaundryReservations { get; set; }
        public DbSet<ServiceMessage> ServiceMessages { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<LostAndFound> LostAndFoundItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define relationships and constraints
            modelBuilder.Entity<Household>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(h => h.UserId);

            modelBuilder.Entity<Address>()
                .HasOne(a => a.Household)
                .WithMany(h => h.Addresses)
                .HasForeignKey(a => a.HouseholdId)
                .OnDelete(DeleteBehavior.Restrict); // Changed to Restrict

            modelBuilder.Entity<Slot>()
                .HasOne(s => s.NotifyHousehold)
                .WithMany(h => h.Slots)
                .HasForeignKey(s => s.NotifyHouseholdId)
                .OnDelete(DeleteBehavior.Restrict); // Changed to Restrict

            modelBuilder.Entity<LaundryReservation>()
                .HasOne(lr => lr.Household)
                .WithMany(h => h.LaundryReservations)
                .HasForeignKey(lr => lr.HouseholdId)
                .OnDelete(DeleteBehavior.Restrict); // Changed to Restrict

            modelBuilder.Entity<LaundryReservation>()
                .HasOne(lr => lr.Slot)
                .WithMany(s => s.LaundryReservations)
                .HasForeignKey(lr => lr.SlotId)
                .OnDelete(DeleteBehavior.Restrict); // Changed to Restrict

            modelBuilder.Entity<ChatMessage>()
                .HasOne(cm => cm.Household)
                .WithMany(h => h.ChatMessages)
                .HasForeignKey(cm => cm.HouseholdId)
                .OnDelete(DeleteBehavior.Restrict); // Changed to Restrict

            modelBuilder.Entity<ChatMessage>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(cm => cm.SenderId)
                .OnDelete(DeleteBehavior.Restrict); // Changed to Restrict

            modelBuilder.Entity<LostAndFound>()
                .HasOne(lf => lf.Household)
                .WithMany(h => h.LostAndFoundItems)
                .HasForeignKey(lf => lf.HouseholdId)
                .OnDelete(DeleteBehavior.Restrict); // Changed to Restrict

            // Seed data for AspNetUsers
            var passwordHasher = new PasswordHasher<IdentityUser>();

            var user1 = new IdentityUser
            {
                Id = "user1",
                UserName = "user1@example.com",
                NormalizedUserName = "USER1@EXAMPLE.COM",
                Email = "user1@example.com",
                NormalizedEmail = "USER1@EXAMPLE.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };
            user1.PasswordHash = passwordHasher.HashPassword(user1, "Password123!");

            var user2 = new IdentityUser
            {
                Id = "user2",
                UserName = "user2@example.com",
                NormalizedUserName = "USER2@EXAMPLE.COM",
                Email = "user2@example.com",
                NormalizedEmail = "USER2@EXAMPLE.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };
            user2.PasswordHash = passwordHasher.HashPassword(user2, "Password123!");

            var user3 = new IdentityUser
            {
                Id = "user3",
                UserName = "user3@example.com",
                NormalizedUserName = "USER3@EXAMPLE.COM",
                Email = "user3@example.com",
                NormalizedEmail = "USER3@EXAMPLE.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };
            user3.PasswordHash = passwordHasher.HashPassword(user3, "Password123!");

            modelBuilder.Entity<IdentityUser>().HasData(user1, user2, user3);

            // Seed Household table
            modelBuilder.Entity<Household>().HasData(
                  new Household { HouseholdId = 1, Name = "Smith Household", UserId = "user1" },
                  new Household { HouseholdId = 2, Name = "Johnson Household", UserId = "user2" },
                  new Household { HouseholdId = 3, Name = "Williams Household", UserId = "user3" }
              );

            // Seed Address table
            modelBuilder.Entity<Address>().HasData(
                new Address { AddressId = 1, HouseholdId = 1, HomeAddress = "123 Main St.", PhoneNumber1 = "555-1234", EmailAddress = "smith@example.com" },
                new Address { AddressId = 2, HouseholdId = 2, HomeAddress = "456 Elm St.", PhoneNumber1 = "555-5678", PhoneNumber2 = "555-9876", EmailAddress = "johnson@example.com" },
                new Address { AddressId = 3, HouseholdId = 3, HomeAddress = "789 Oak St.", PhoneNumber1 = "555-4321", EmailAddress = "williams@example.com" }
            );

            // Seed Slots table
            modelBuilder.Entity<Slot>().HasData(
                new Slot { SlotId = 1, AvailableDateTime = DateTime.Now.AddDays(1), Reserved = false, NotifyHouseholdId = 1 },
                new Slot { SlotId = 2, AvailableDateTime = DateTime.Now.AddDays(2), Reserved = false, NotifyHouseholdId = 2 },
                new Slot { SlotId = 3, AvailableDateTime = DateTime.Now.AddDays(3), Reserved = false, NotifyHouseholdId = 3 }
            );

            // Seed LaundryReservation table
            modelBuilder.Entity<LaundryReservation>().HasData(
                new LaundryReservation { ReservationId = 1, HouseholdId = 1, SlotId = 1, ExpectedStart = new TimeSpan(10, 0, 0) },
                new LaundryReservation { ReservationId = 2, HouseholdId = 2, SlotId = 2, ExpectedStart = new TimeSpan(12, 0, 0) },
                new LaundryReservation { ReservationId = 3, HouseholdId = 3, SlotId = 3, ExpectedStart = new TimeSpan(14, 0, 0) }
            );

            // Seed ServiceMessage table
            modelBuilder.Entity<ServiceMessage>().HasData(
                new ServiceMessage { ServiceMessageId = 1, Message = "Welcome to LaundrySystem!" },
                new ServiceMessage { ServiceMessageId = 2, Message = "Please adhere to the laundry schedule." },
                new ServiceMessage { ServiceMessageId = 3, Message = "Thank you for using LaundrySystem." }
            );
        }

        public void SetModified<TEntity>(TEntity entity) where TEntity : class
        {
            Entry(entity).State = EntityState.Modified;
        }
    }
}