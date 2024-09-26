using LaundrySystem.Domain.Model.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace LaundrySystem.DAL.DataModel
{
    public class DataContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<ServiceMessage> ServiceMessages { get; set; }
        public DbSet<LostAndFound> LostAndFoundItems { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Timeslot> Timeslots { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<DesiredTimeslot> DesiredTimeslots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations from the current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}