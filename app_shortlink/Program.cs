using app_shortlink.DAL;
using app_shortlink.DAL.Repository;
using app_shortlink.DAL.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

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

app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
app.UseEndpoints(x => x.MapControllers());



 
 

app.Run();

