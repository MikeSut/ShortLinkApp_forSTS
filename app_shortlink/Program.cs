using app_shortlink.DAL;
using app_shortlink.DAL.Repository;
using app_shortlink.DAL.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

var build = new ConfigurationBuilder();
build.SetBasePath(Directory.GetCurrentDirectory());
build.AddJsonFile("appsettings.json");
var config = build.Build();
var connectionString = config.GetConnectionString("DefaultConnection");
var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
var options = optionsBuilder.UseNpgsql(connectionString).Options;
using (ApplicationDbContext db = new ApplicationDbContext(options))
{
    var users = db.Users;
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUserRepository, UserRepository>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
 builder.Services.AddEndpointsApiExplorer();
 builder.Services.AddSwaggerGen();


 var app = builder.Build();



 // Configure the HTTP request pipeline.
 if (app.Environment.IsDevelopment())
 {
     app.UseSwagger();
     app.UseSwaggerUI();
 }

 app.UseHttpsRedirection();

 var summaries = new[]
 {
     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
 };

 app.MapGet("/weatherforecast", () =>
     {
         var forecast = Enumerable.Range(1, 5).Select(index =>
                 new WeatherForecast
                 (
                     DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                     Random.Shared.Next(-20, 55),
                     summaries[Random.Shared.Next(summaries.Length)]
                 ))
             .ToArray();
         return forecast;
     })
     .WithName("GetWeatherForecast")
     .WithOpenApi();

app.Run();

 record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
 {
     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
 }