using app_shortlink.DAL;
using app_shortlink.DAL.Repository;
using app_shortlink.DAL.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

// var contact = new ConfigurationBuilder();
// contact.SetBasePath(Directory.GetCurrentDirectory());
// contact.AddJsonFile("appsettings.json");
// var config = contact.Build();
// var connectionString = config.GetConnectionString("DefaultConnection");
// var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
// var options = optionsBuilder.UseNpgsql(connectionString).Options;
// using (ApplicationDbContext db = new ApplicationDbContext(options)) 
// {
//     var users = db.Users;
// }

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddControllers();


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