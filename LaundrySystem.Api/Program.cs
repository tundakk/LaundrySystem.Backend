using Hangfire;
using Hangfire.PostgreSql;
using LaundrySystem.Api.BackgroundServices;
using LaundrySystem.API;
using LaundrySystem.BLL;
using LaundrySystem.DAL.DataModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File(
        path: "logs/laundrysystem-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting LaundrySystem API");
    builder.Host.UseSerilog();

    builder.Services.AddControllers()
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                var errors = context.ModelState
                    .Where(x => x.Value!.Errors.Count > 0)
                    .SelectMany(x => x.Value!.Errors.Select(e => $"{x.Key}: {e.ErrorMessage}"))
                    .ToList();

                logger.LogWarning(
                    "Model Validation Failed - {Method} {Path}: {Errors}",
                    context.HttpContext.Request.Method,
                    context.HttpContext.Request.Path,
                    string.Join(" | ", errors));

                return new BadRequestObjectResult(new { errors = errors });
            };
        });

    // Configure DbContext
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    builder.Services.AddDbContext<DataContext>(options =>
        options.UseNpgsql(connectionString));

    // Configure Identity
    builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
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
    var secretKey = builder.Configuration["Jwt:SecretKey"]
        ?? throw new InvalidOperationException("JWT Secret Key is not configured.");
    var key = Encoding.ASCII.GetBytes(secretKey);

    builder.Services.AddAuthentication(options =>
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
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"]
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

    // Add Business Logic Layer services (repositories and services only)
    builder.Services.AddBusinessLogicLayer(builder.Configuration);

    // Configure Hangfire with PostgreSQL storage
    builder.Services.AddHangfire(config => config
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(options =>
            options.UseNpgsqlConnection(connectionString)));
    builder.Services.AddHangfireServer();

    // Register background job classes for DI
    builder.Services.AddScoped<BookingReminderJob>();
    builder.Services.AddScoped<SubscriptionExpiryJob>();
    builder.Services.AddScoped<LostAndFoundCleanupJob>();

    // Register Mapster mappings
    MappingConfig.RegisterMappings();

    // Configure Swagger
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "LaundrySystem API", Version = "v1" });

        // Add JWT Authentication to Swagger
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = @"JWT Authorization header using the Bearer scheme.
                          Enter 'Bearer' [space] and then your token in the text input below.
                          Example: 'Bearer 12345abcdef'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                  new OpenApiSecurityScheme
                  {
                      Reference = new OpenApiReference
                      {
                          Type = ReferenceType.SecurityScheme,
                          Id = "Bearer"
                      },
                      Scheme = "Bearer",
                      Name = "Bearer",
                      In = ParameterLocation.Header,
                  },
                  new List<string>()
            }
        });
    });

    // CORS configuration
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowLocalhost",
            policyBuilder =>
            {
                policyBuilder.WithOrigins("http://localhost:3000", "http://localhost:3001", "exp://192.168.153.179:8081")
                             .AllowAnyHeader()
                             .AllowAnyMethod()
                             .AllowCredentials();
            });
    });

    var app = builder.Build();

    // Seed data
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            await SeedData.InitializeAsync(services);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseCors("AllowLocalhost");

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    // Hangfire dashboard (admin-only)
    app.MapHangfireDashboard("/hangfire", new Hangfire.DashboardOptions
    {
        Authorization = new[] { new HangfireDashboardAuthFilter() },
        DashboardTitle = "LaundrySystem Jobs"
    });

    // Register Hangfire recurring jobs
    var jobConfig = builder.Configuration.GetSection("BackgroundJobs");
    var reminderInterval = jobConfig.GetValue("BookingReminderCheckIntervalMinutes", 15);

    RecurringJob.AddOrUpdate<BookingReminderJob>(
        "booking-reminders",
        job => job.ExecuteAsync(),
        $"*/{reminderInterval} * * * *");

    RecurringJob.AddOrUpdate<SubscriptionExpiryJob>(
        "subscription-expiry",
        job => job.ExecuteAsync(),
        Cron.Daily(3, 0));

    RecurringJob.AddOrUpdate<LostAndFoundCleanupJob>(
        "lost-found-cleanup",
        job => job.ExecuteAsync(),
        Cron.Weekly(DayOfWeek.Sunday, 4, 0));

    // If you have identity endpoints, make sure to map them
    app.MapIdentityApi<AppUser>();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unhandled exception occurred during startup");
}
finally
{
    Log.Information("LaundrySystem API has shut down");
    await Log.CloseAndFlushAsync();
}