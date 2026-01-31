using System.Reflection;
using System.Security.Claims;
using LaundrySystem.Domain.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LaundrySystem.DAL.DataModel;

public class DataContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    private readonly IHttpContextAccessor? _httpContextAccessor;

    // Constructor for design-time migrations (no HTTP context)
    public DataContext(DbContextOptions<DataContext> options) : base(options)
    {
    }

    // Constructor for runtime with HTTP context (tenant filtering)
    public DataContext(
        DbContextOptions<DataContext> options,
        IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    // Multi-tenancy entities
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Building> Buildings { get; set; }
    public DbSet<AccountSettings> AccountSettings { get; set; }
    public DbSet<BuildingSettings> BuildingSettings { get; set; }

    // Existing entities
    public DbSet<ServiceMessage> ServiceMessages { get; set; }
    public DbSet<LostAndFound> LostAndFoundItems { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Timeslot> Timeslots { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<DesiredTimeslot> DesiredTimeslots { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    // Tenant context helper properties (extracted from JWT claims)
    // Using IHttpContextAccessor directly avoids circular dependency with ITenantContext
    private Guid CurrentAccountId =>
        Guid.TryParse(_httpContextAccessor?.HttpContext?.User
            .FindFirst("account_id")?.Value, out var id) ? id : Guid.Empty;

    private Guid CurrentBuildingId =>
        Guid.TryParse(_httpContextAccessor?.HttpContext?.User
            .FindFirst("building_id")?.Value, out var id) ? id : Guid.Empty;

    private bool IsAccountAdmin =>
        _httpContextAccessor?.HttpContext?.User.IsInRole("AccountAdmin") ?? false;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Apply tenant isolation filters
        ConfigureTenantFilters(modelBuilder);
    }

    /// <summary>
    /// Configures global query filters for multi-tenant isolation.
    /// Filters are conditional: only applied when tenant context exists.
    /// AccountAdmin can access all buildings within their account.
    /// </summary>
    private void ConfigureTenantFilters(ModelBuilder modelBuilder)
    {
        // Building: Account-level filter (AccountAdmins see all buildings in their account)
        modelBuilder.Entity<Building>().HasQueryFilter(b =>
            CurrentAccountId == Guid.Empty || b.AccountId == CurrentAccountId);

        // Room: Building-level with AccountAdmin exception
        // Regular users see only their building's rooms
        // AccountAdmins see all rooms in their account
        modelBuilder.Entity<Room>().HasQueryFilter(r =>
            CurrentAccountId == Guid.Empty ||
            (r.Building.AccountId == CurrentAccountId &&
             (IsAccountAdmin || r.BuildingId == CurrentBuildingId)));

        // Timeslot: Filtered through Room → Building chain
        modelBuilder.Entity<Timeslot>().HasQueryFilter(t =>
            CurrentAccountId == Guid.Empty ||
            (t.Room.Building.AccountId == CurrentAccountId &&
             (IsAccountAdmin || t.Room.BuildingId == CurrentBuildingId)));

        // AppUser: Building-level with AccountAdmin exception
        // Note: This DOES affect Identity queries, but filter is conditional
        modelBuilder.Entity<AppUser>().HasQueryFilter(u =>
            CurrentAccountId == Guid.Empty ||
            (u.Building.AccountId == CurrentAccountId &&
             (IsAccountAdmin || u.BuildingId == CurrentBuildingId)));

        // Booking: Filtered through User → Building chain
        modelBuilder.Entity<Booking>().HasQueryFilter(b =>
            CurrentAccountId == Guid.Empty ||
            (b.User.Building.AccountId == CurrentAccountId &&
             (IsAccountAdmin || b.User.BuildingId == CurrentBuildingId)));

        // DesiredTimeslot: Filtered through User → Building chain
        modelBuilder.Entity<DesiredTimeslot>().HasQueryFilter(dt =>
            CurrentAccountId == Guid.Empty ||
            (dt.User.Building.AccountId == CurrentAccountId &&
             (IsAccountAdmin || dt.User.BuildingId == CurrentBuildingId)));

        // LostAndFound: Building-level with AccountAdmin exception
        modelBuilder.Entity<LostAndFound>().HasQueryFilter(lf =>
            CurrentAccountId == Guid.Empty ||
            (lf.Building.AccountId == CurrentAccountId &&
             (IsAccountAdmin || lf.BuildingId == CurrentBuildingId)));

        // Notification: Filtered through User → Building chain
        modelBuilder.Entity<Notification>().HasQueryFilter(n =>
            CurrentAccountId == Guid.Empty ||
            (n.User.Building.AccountId == CurrentAccountId &&
             (IsAccountAdmin || n.User.BuildingId == CurrentBuildingId)));

        // ServiceMessage: Account-level with optional building scope
        // If BuildingId is null, message applies to all buildings (account-wide)
        // Otherwise, filter by building (or show all if AccountAdmin)
        modelBuilder.Entity<ServiceMessage>().HasQueryFilter(sm =>
            CurrentAccountId == Guid.Empty ||
            (sm.AccountId == CurrentAccountId &&
             (sm.BuildingId == null || IsAccountAdmin || sm.BuildingId == CurrentBuildingId)));
    }
}
