using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurementAppBusinessLayer.Interface;
using QuantityMeasurementAppBusinessLayer.Service;
using QuantityMeasurementAppModelLayer.Utils;
using QuantityMeasurementAppRepositoryLayer.Data;
using QuantityMeasurementAppRepositoryLayer.Database;
using QuantityMeasurementAppRepositoryLayer.Interface;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── Core Services ──────────────────────────────────────────────────────────
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});


// ── JWT Settings ───────────────────────────────────────────────────────────
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("Jwt").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);

// ── Entity Framework Core – SQL Server ────────────────────────────────────
// DbContext is registered here in the API layer and injected into the
// repository. No more static ConnectionConfig or raw SqlConnection.
builder.Services.AddDbContext<QuantityMeasurementDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ── JWT Authentication ─────────────────────────────────────────────────────
builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer              = jwtSettings.Issuer,
        ValidAudience            = jwtSettings.Audience,
        IssuerSigningKey         = new SymmetricSecurityKey(
                                       Encoding.UTF8.GetBytes(jwtSettings.Key!))
    };
});

builder.Services.AddAuthorization();

// ── Dependency Injection ───────────────────────────────────────────────────
builder.Services.AddScoped<IQuantityMeasurementRepository, QuantityMeasurementDatabaseRepository>();
builder.Services.AddScoped<IAuthService, QuantityMeasurementAuthService>();
builder.Services.AddScoped<IQuantityMeasurementService, QuantityMeasurementService>();

// ── Build ──────────────────────────────────────────────────────────────────
var app = builder.Build();
app.UseCors("AllowAll");

// Auto-apply EF Core migrations on startup (creates the DB/tables if needed)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<QuantityMeasurementDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Migration failed: " + ex.Message);
    }
}

// ── Pipeline ───────────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();