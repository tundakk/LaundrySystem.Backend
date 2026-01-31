using LaundrySystem.BLL.FileStorage;
using LaundrySystem.BLL.Infrastructure.Interfaces;
using LaundrySystem.BLL.Infrastructure.Services.Implementations;
using LaundrySystem.BLL.Multitenancy;
using LaundrySystem.BLL.Notifications;
using LaundrySystem.BLL.SMS;
using LaundrySystem.DAL.DataModel;
using LaundrySystem.DAL.Repos;
using LaundrySystem.DAL.Repos.Implementations;
using LaundrySystem.DAL.Repos.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LaundrySystem.BLL
{
    /// <summary>
    /// Extension methods for setting up business logic layer services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the business logic layer services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <param name="configuration">The configuration instance.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // Register multi-tenancy services
            services.AddHttpContextAccessor();
            services.AddScoped<ITenantContext, TenantContext>();
            services.AddScoped<ISettingsResolver, SettingsResolver>();

            // Register repositories (DAL)
            services.AddScoped<IAccountRepo, AccountRepo>();
            services.AddScoped<IBuildingRepo, BuildingRepo>();
            services.AddScoped<IAccountSettingsRepo, AccountSettingsRepo>();
            services.AddScoped<IBuildingSettingsRepo, BuildingSettingsRepo>();
            services.AddScoped<IAppUserRepo, AppUserRepo>();
            services.AddScoped<IBookingRepo, BookingRepo>();
            services.AddScoped<IDesiredTimeslotRepo, DesiredTimeslotRepo>();
            services.AddScoped<ILostAndFoundRepo, LostAndFoundRepo>();
            services.AddScoped<IServiceMessageRepo, ServiceMessageRepo>();
            services.AddScoped<IRoomRepo, RoomRepo>();
            services.AddScoped<ITimeslotRepo, TimeslotRepo>();
            services.AddScoped<INotificationRepo, NotificationRepo>();

            // Register services (BLL)
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IBuildingService, BuildingService>();
            services.AddScoped<IAccountSettingsService, AccountSettingsService>();
            services.AddScoped<IBuildingSettingsService, BuildingSettingsService>();
            services.AddScoped<IUserSettingsService, UserSettingsService>();
            services.AddScoped<IAppUserService, AppUserService>();
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IDesiredTimeslotService, DesiredTimeslotService>();
            services.AddScoped<ILostAndFoundService, LostAndFoundService>();
            services.AddScoped<IServiceMessageService, ServiceMessageService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<ITimeslotService, TimeslotService>();
            services.AddScoped<IInAppNotificationService, InAppNotificationService>();

            // Register Twilio SMS service
            services.Configure<TwilioSettings>(configuration.GetSection("Twilio"));
            services.AddScoped<ISMSService, SMSService>();

            // Register Email Sender Service
            services.AddTransient<IEmailSender<AppUser>, BrevoEmailSender>();

            // Register File Storage Service
            services.Configure<FileStorageSettings>(configuration.GetSection("FileStorage"));
            services.AddScoped<IFileStorageService, LocalFileStorageService>();

            // Register Notification Service
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
}