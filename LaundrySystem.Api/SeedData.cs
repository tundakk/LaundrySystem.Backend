// SeedData.cs
using LaundrySystem.DAL.DataModel;
using LaundrySystem.Domain.Model.Entities;
using LaundrySystem.Domain.Model.Enums;
using LaundrySystem.Domain.Model.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LaundrySystem.API;

/// <summary>
/// Provides methods to seed initial data into the database.
/// </summary>
public static class SeedData
{
    // Fixed IDs for consistent seeding
    private static readonly Guid AccountId = Guid.Parse("a0000000-0000-0000-0000-000000000001");
    private static readonly Guid Building1Id = Guid.Parse("b0000000-0000-0000-0000-000000000001");
    private static readonly Guid Building2Id = Guid.Parse("b0000000-0000-0000-0000-000000000002");
    private static readonly Guid AccountSettingsId = Guid.Parse("c0000000-0000-0000-0000-000000000001");

    /// <summary>
    /// Initializes the database with seed data.
    /// </summary>
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        var context = serviceProvider.GetRequiredService<DataContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        try
        {
            // Ensure database is created
            await context.Database.MigrateAsync();

            // Seed in order of dependencies
            await SeedRolesAsync(roleManager);
            var account = await SeedAccountAsync(context);
            var buildings = await SeedBuildingsAsync(context, account.Id);
            await SeedAccountSettingsAsync(context, account.Id);
            var users = await SeedUsersAsync(userManager, buildings);
            var rooms = await SeedRoomsAsync(context, buildings);
            var timeslots = await SeedTimeslotsAsync(context, rooms);
            await SeedServiceMessagesAsync(context, account.Id);
            await SeedBookingsAsync(context, users, timeslots);

            await context.SaveChangesAsync();
            logger.LogInformation("Database seeded successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        var roles = new[] { "User", "AccountAdmin", "SuperAdmin" };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                });
            }
        }
    }

    private static async Task<Account> SeedAccountAsync(DataContext context)
    {
        var existing = await context.Accounts.FindAsync(AccountId);
        if (existing != null) return existing;

        var account = new Account
        {
            Id = AccountId,
            Name = "Test Boligforening",
            OrganizationNumber = "12345678", // CVR
            ContactEmail = "admin@testbolig.dk",
            ContactPhone = "+45 12345678",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Accounts.Add(account);
        await context.SaveChangesAsync();
        return account;
    }

    private static async Task<List<Building>> SeedBuildingsAsync(DataContext context, Guid accountId)
    {
        var buildings = new List<Building>();

        var building1 = await context.Buildings.FindAsync(Building1Id);
        if (building1 == null)
        {
            building1 = new Building
            {
                Id = Building1Id,
                AccountId = accountId,
                Name = "Blok A",
                Address = "Testvej 1",
                PostalCode = "2100",
                City = "Copenhagen",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Buildings.Add(building1);
        }
        buildings.Add(building1);

        var building2 = await context.Buildings.FindAsync(Building2Id);
        if (building2 == null)
        {
            building2 = new Building
            {
                Id = Building2Id,
                AccountId = accountId,
                Name = "Blok B",
                Address = "Testvej 3",
                PostalCode = "2100",
                City = "Copenhagen",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Buildings.Add(building2);
        }
        buildings.Add(building2);

        await context.SaveChangesAsync();
        return buildings;
    }

    private static async Task SeedAccountSettingsAsync(DataContext context, Guid accountId)
    {
        var existing = await context.AccountSettings.FirstOrDefaultAsync(s => s.AccountId == accountId);
        if (existing != null) return;

        var settings = new AccountSettings
        {
            Id = AccountSettingsId,
            AccountId = accountId,
            DefaultOperatingHours = new OperatingHours
            {
                OpenTime = new TimeOnly(6, 0),
                CloseTime = new TimeOnly(22, 0),
                OperatingDays = new List<DayOfWeek>
                {
                    DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday,
                    DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday
                }
            },
            MaxAdvanceBookingDays = 14,
            SlotDurationMinutes = 60,
            MaxActiveBookingsPerUser = 2,
            CancellationDeadlineMinutes = 60,
            DefaultEmailNotifications = true,
            DefaultSmsNotifications = false,
            ReminderHoursBefore = 24
        };

        context.AccountSettings.Add(settings);
        await context.SaveChangesAsync();
    }

    private static async Task<List<AppUser>> SeedUsersAsync(UserManager<AppUser> userManager, List<Building> buildings)
    {
        // Define user data (separate from collection building to avoid modification issues)
        var usersToCreate = new (AppUser user, string role)[]
        {
            // Admin user in Building 1
            (new AppUser
            {
                Id = Guid.Parse("115b5117-73f6-4796-a87a-962181baa3e5"),
                UserName = "admin@testbolig.dk",
                Email = "admin@testbolig.dk",
                NormalizedUserName = "ADMIN@TESTBOLIG.DK",
                NormalizedEmail = "ADMIN@TESTBOLIG.DK",
                EmailConfirmed = true,
                BuildingId = buildings[0].Id,
                ApartmentNumber = 101,
                PhoneNumber = "+45 11111111",
                PinCode = 1234,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Settings = new UserSettings { PreferredLanguage = "da" }
            }, "AccountAdmin"),

            // Regular user in Building 1
            (new AppUser
            {
                Id = Guid.Parse("225b5117-73f6-4796-a87a-962181baa3e5"),
                UserName = "user1@testbolig.dk",
                Email = "user1@testbolig.dk",
                NormalizedUserName = "USER1@TESTBOLIG.DK",
                NormalizedEmail = "USER1@TESTBOLIG.DK",
                EmailConfirmed = true,
                BuildingId = buildings[0].Id,
                ApartmentNumber = 102,
                PhoneNumber = "+45 22222222",
                PinCode = 5678,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Settings = new UserSettings { PreferredLanguage = "da" }
            }, "User"),

            // Regular user in Building 2
            (new AppUser
            {
                Id = Guid.Parse("335b5117-73f6-4796-a87a-962181baa3e5"),
                UserName = "user2@testbolig.dk",
                Email = "user2@testbolig.dk",
                NormalizedUserName = "USER2@TESTBOLIG.DK",
                NormalizedEmail = "USER2@TESTBOLIG.DK",
                EmailConfirmed = true,
                BuildingId = buildings[1].Id,
                ApartmentNumber = 201,
                PhoneNumber = "+45 33333333",
                PinCode = 9012,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Settings = new UserSettings { PreferredLanguage = "da" }
            }, "User")
        };

        // First pass: create all users
        var createdUserIds = new List<Guid>();
        for (int i = 0; i < usersToCreate.Length; i++)
        {
            var (user, role) = usersToCreate[i];
            var existing = await userManager.FindByEmailAsync(user.Email!);
            if (existing == null)
            {
                var result = await userManager.CreateAsync(user, "Password123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                    createdUserIds.Add(user.Id);
                }
                else
                {
                    throw new Exception($"Failed to create user {user.Email}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                createdUserIds.Add(existing.Id);
            }
        }

        // Second pass: fetch all users by their IDs to return a clean list
        var users = new List<AppUser>();
        foreach (var userId in createdUserIds)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                users.Add(user);
            }
        }

        return users;
    }

    private static async Task<List<Room>> SeedRoomsAsync(DataContext context, List<Building> buildings)
    {
        var rooms = new List<Room>();

        // Room in Building 1
        var room1Id = Guid.Parse("0c9c62e6-2d71-4a7d-b38e-fd3d9f8c0a6f");
        var room1 = await context.Rooms.FindAsync(room1Id);
        if (room1 == null)
        {
            room1 = new Room
            {
                Id = room1Id,
                BuildingId = buildings[0].Id,
                Name = "Vaskekælder A",
                Location = "Kælderen",
                IsAvailable = true,
                MaxCapacity = 3
            };
            context.Rooms.Add(room1);
        }
        rooms.Add(room1);

        // Room in Building 2
        var room2Id = Guid.Parse("1d8e57f9-5e9b-4c4b-9f2d-1c4e7a1e2b2c");
        var room2 = await context.Rooms.FindAsync(room2Id);
        if (room2 == null)
        {
            room2 = new Room
            {
                Id = room2Id,
                BuildingId = buildings[1].Id,
                Name = "Vaskekælder B",
                Location = "Kælderen",
                IsAvailable = true,
                MaxCapacity = 2
            };
            context.Rooms.Add(room2);
        }
        rooms.Add(room2);

        await context.SaveChangesAsync();
        return rooms;
    }

    private static async Task<List<Timeslot>> SeedTimeslotsAsync(DataContext context, List<Room> rooms)
    {
        var timeslots = new List<Timeslot>();

        var timeslot1Id = Guid.Parse("2e7c8c0d-9f6b-4e4f-9f4b-2a1d9f6b4e4f");
        var timeslot1 = await context.Timeslots.FindAsync(timeslot1Id);
        if (timeslot1 == null)
        {
            timeslot1 = new Timeslot
            {
                Id = timeslot1Id,
                RoomId = rooms[0].Id,
                SlotTime = new TimeRange(DateTime.UtcNow.Date.AddHours(8), DateTime.UtcNow.Date.AddHours(9)),
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Timeslots.Add(timeslot1);
        }
        timeslots.Add(timeslot1);

        var timeslot2Id = Guid.Parse("3f8d9d1e-af7c-5d5e-0e5c-3b2e0e7c5f5e");
        var timeslot2 = await context.Timeslots.FindAsync(timeslot2Id);
        if (timeslot2 == null)
        {
            timeslot2 = new Timeslot
            {
                Id = timeslot2Id,
                RoomId = rooms[1].Id,
                SlotTime = new TimeRange(DateTime.UtcNow.Date.AddHours(10), DateTime.UtcNow.Date.AddHours(11)),
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            context.Timeslots.Add(timeslot2);
        }
        timeslots.Add(timeslot2);

        await context.SaveChangesAsync();
        return timeslots;
    }

    private static async Task SeedServiceMessagesAsync(DataContext context, Guid accountId)
    {
        if (await context.ServiceMessages.AnyAsync()) return;

        var messages = new List<ServiceMessage>
        {
            new ServiceMessage
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                BuildingId = null,
                Title = "Velkommen",
                Body = "Velkommen til LaundrySystem!",
                Severity = ServiceMessageSeverity.Info,
                ActiveFrom = DateTime.UtcNow,
                ActiveTo = null,
                CreatedAt = DateTime.UtcNow
            },
            new ServiceMessage
            {
                Id = Guid.NewGuid(),
                AccountId = accountId,
                BuildingId = null,
                Title = "Vasketider",
                Body = "Husk at overholde vasketiderne.",
                Severity = ServiceMessageSeverity.Warning,
                ActiveFrom = DateTime.UtcNow,
                ActiveTo = null,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.ServiceMessages.AddRangeAsync(messages);
        await context.SaveChangesAsync();
    }

    private static async Task SeedBookingsAsync(DataContext context, List<AppUser> users, List<Timeslot> timeslots)
    {
        if (await context.Bookings.AnyAsync()) return;

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            UserId = users[0].Id,
            TimeslotId = timeslots[0].Id,
            Status = BookingStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        context.Bookings.Add(booking);
        await context.SaveChangesAsync();
    }
}
