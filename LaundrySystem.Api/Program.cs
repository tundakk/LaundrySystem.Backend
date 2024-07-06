using LaundrySystem.BLL.Infrastructure.Services;
using LaundrySystem.BLL.Services.Interfaces;
using LaundrySystem.BLL.SMS;
using LaundrySystem.DAL.DataModel;
using LaundrySystem.DAL.Repos;
using LaundrySystem.DAL.Repos.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString));

// Register repositories
builder.Services.AddScoped<IAddressRepo, AddressRepo>();
builder.Services.AddScoped<IChatMessageRepo, ChatMessageRepo>();
builder.Services.AddScoped<IHouseholdRepo, HouseholdRepo>();
builder.Services.AddScoped<ILaundryReservationRepo, LaundryReservationRepo>();
builder.Services.AddScoped<ILostAndFoundRepo, LostAndFoundRepo>();
builder.Services.AddScoped<IServiceMessageRepo, ServiceMessageRepo>();
builder.Services.AddScoped<ISlotRepo, SlotRepo>();

// Register services
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IChatMessageService, ChatMessageService>();
builder.Services.AddScoped<IHouseholdService, HouseholdService>();
builder.Services.AddScoped<ILaundryReservationService, LaundryReservationService>();
builder.Services.AddScoped<ILostAndFoundService, LostAndFoundService>();
builder.Services.AddScoped<IServiceMessageService, ServiceMessageService>();
builder.Services.AddScoped<ISlotService, SlotService>();

// Register Twilio SMS service
builder.Services.AddSingleton<ISMSService>(new SMSService(
    builder.Configuration["Twilio:AccountSid"],
    builder.Configuration["Twilio:AuthToken"],
    builder.Configuration["Twilio:FromPhoneNumber"]
));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<DataContext>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register Mapster mappings
MappingConfig.RegisterMappings();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.Run();