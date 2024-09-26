using LaundrySystem.API;
using LaundrySystem.BLL;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add Business Logic Layer services, including DbContext, Identity, Repositories, and other services.
builder.Services.AddBusinessLogicLayer(builder.Configuration);

// Register Mapster mappings
MappingConfig.RegisterMappings();

////LOGGING
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();

//builder.Logging.SetMinimumLevel(LogLevel.Error);
//builder.WebHost.CaptureStartupErrors(true).UseSetting("detailedErrors", "true");
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
    // If you need MigrationsEndPoint, ensure you have added the necessary services
    // app.UseMigrationsEndPoint();
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

// If you have identity endpoints, make sure to map them
app.MapIdentityApi<AppUser>();

app.Run();