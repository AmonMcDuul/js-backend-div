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

builder.Services.AddDbContext<JsDbContext>(
    options => options
    .UseSqlServer(builder.Configuration.GetConnectionString("database")));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<IHighScoreCacheService, HighScoreCacheService>();
builder.Services.AddScoped<IHighScoreSyncService, HighScoreSyncService>();
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

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    JsDbContext dbContext = services.GetRequiredService<JsDbContext>();

//    if (dbContext.Database.IsSqlServer())
//    {
//        if (dbContext.Database.GetPendingMigrations().Any())
//        {
//            string query = "USE master; ALTER DATABASE [jsdivers] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [jsdivers];";
//            try
//            {
//                await dbContext.Database.ExecuteSqlRawAsync(query);
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine("Database not deleted because it didn't exist" + e);
//            }
//            dbContext.Database.Migrate();
//        }
//    }
//}

app.UseRouting();

app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
