using Core.Entities.EmailModels;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dbPath = builder.Environment.IsDevelopment()
    ? Path.Combine("Data", "highscores.db") 
    : Path.Combine(
        Environment.GetEnvironmentVariable("HOME") ?? string.Empty,
        "data", 
        "highscores.db");

// Ensure directory exists
var directory = Path.GetDirectoryName(dbPath);
if (!Directory.Exists(directory) && !string.IsNullOrEmpty(directory))
{
    Directory.CreateDirectory(directory);
}

// Register DbContext
builder.Services.AddDbContext<JsDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.AllowAnyMethod().AllowAnyHeader().WithOrigins(
                "https://amonmcduul.github.io", "https://scholsdev.azurewebsites.net", "https://flamsoft.nl", "https://flamsoft.netlify.app", "https://flamsoft.info")
            .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors(builder =>
    {
        // this should be configured on the host. See App service > CORS. Values set here must only be used for localhost.
        builder.AllowAnyMethod().AllowAnyHeader().WithOrigins(
            "http://localhost:4200", "http://localhost:4201")
        .AllowCredentials();
    });
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var dbContext = services.GetRequiredService<JsDbContext>();

    try
    {
        if (dbContext.Database.IsSqlite())
        {
            logger.LogInformation("Applying migrations...");
            dbContext.Database.Migrate();
            logger.LogInformation("Migrations applied successfully");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database");
        throw;
    }
}

app.UseRouting();

app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
