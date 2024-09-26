using LaundrySystem.BLL.Infrastructure.Interfaces;
using LaundrySystem.BLL.Infrastructure.Services.Implementations;
using LaundrySystem.BLL.SMS;
using LaundrySystem.DAL.DataModel;
using LaundrySystem.DAL.Repos;
using LaundrySystem.DAL.Repos.Implementations;
using LaundrySystem.DAL.Repos.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace LaundrySystem.BLL
{
    /// <summary>
    /// Extension methods for setting up services in the business logic layer and setting up repositories in the DAL layer.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the business logic layer services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The updated service collection.</returns>
        /// <exception cref="InvalidOperationException">Thrown when a required configuration value is missing.</exception>
        public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(connectionString));

            // Register Identity services
            services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

            // Configure JWT authentication
            var secretKey = configuration["Jwt:SecretKey"]
                ?? throw new InvalidOperationException("JWT Secret Key is not configured.");
            var key = Encoding.ASCII.GetBytes(secretKey); // Secret key stored in configuration

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false; // Set to true in production
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero, // Adjust skew tolerance
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"]
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<JwtBearerEvents>>();
                        logger.LogError("Authentication failed: {Message}", context.Exception.Message);
                        context.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    }
                };
            });

            // Register repositories (DAL)
            services.AddScoped<IAppUserRepo, AppUserRepo>();
            services.AddScoped<IBookingRepo, BookingRepo>();
            services.AddScoped<IDesiredTimeslotRepo, DesiredTimeslotRepo>();
            services.AddScoped<ILostAndFoundRepo, LostAndFoundRepo>();
            services.AddScoped<IServiceMessageRepo, ServiceMessageRepo>();
            services.AddScoped<IRoomRepo, RoomRepo>();
            services.AddScoped<ITimeslotRepo, TimeslotRepo>();

            // Register services (BLL)
            services.AddScoped<IAppUserService, AppUserService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IDesiredTimeslotService, DesiredTimeslotService>();
            services.AddScoped<ILostAndFoundService, LostAndFoundService>();
            services.AddScoped<IServiceMessageService, ServiceMessageService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<ITimeslotService, TimeslotService>();

            // Register Twilio SMS service
            services.Configure<TwilioSettings>(configuration.GetSection("Twilio"));
            services.AddTransient<ISMSService, SMSService>();

            // Register Email Sender Service
            services.AddTransient<IEmailSender<AppUser>, BrevoEmailSender>();

            return services;
        }
    }
}