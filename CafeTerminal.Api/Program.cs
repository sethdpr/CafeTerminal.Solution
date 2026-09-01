using CafeTerminal.Api.Data;
using CafeTerminal.Api.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter your JWT token. Example: Bearer {token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
}); //Swagger for API testing

// Register the SQL Server EF Core context used by Identity and the app data.
builder.Services.AddDbContext<CafeTerminalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); //Gets the connection string from appsettings.json

// Register ASP.NET Core Identity for user registration and login.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<CafeTerminalDbContext>()
    .AddDefaultTokenProviders(); //Identity setup for user management

// JWT authentication setup
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtKey = builder.Configuration["Jwt:Key"];

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey!)),

        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],

        ValidateLifetime = true,

        ClockSkew = TimeSpan.Zero
    };
});

// Enable authorization policies that build on the JWT authentication setup.
builder.Services.AddAuthorization();

// Register custom application services used by controllers and startup initialization.
builder.Services.AddScoped<JwtService>(); //If a controller needs to generate a JWT token, it can use this service
builder.Services.AddScoped<ITableService, TableService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Build the configured ASP.NET Core app.
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // Resolve the database context inside a startup scope.
    var db = scope.ServiceProvider.GetRequiredService<CafeTerminalDbContext>();
    try
    {
        // Apply EF Core migrations (preferred)
        db.Database.Migrate();
    }
    catch
    {
        // If migrations cannot be applied for some reason, fall back to EnsureCreated
        db.Database.EnsureCreated();
    }

    // Run manual initialization steps that keep older databases compatible.
    // Also call InitializeAsync on services to ensure any legacy databases get required tables/columns
    var tableService = scope.ServiceProvider.GetRequiredService<ITableService>();
    tableService.InitializeAsync().GetAwaiter().GetResult();
    var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
    productService.InitializeAsync().GetAwaiter().GetResult();
    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
    orderService.InitializeAsync().GetAwaiter().GetResult();
}

// Enable Swagger UI only during development.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CafeTerminal API v1");
        c.RoutePrefix = "swagger";
    });
}

// Redirect HTTP traffic to HTTPS outside development.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Enable authentication before authorization so JWT identities are available.
app.UseAuthentication();
app.UseAuthorization();

// Map controller routes and start the web host.
app.MapControllers();

app.Run();