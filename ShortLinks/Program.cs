using ShortLinks.Application;
using Microsoft.EntityFrameworkCore;
using ShortLinks.Application.Services;
using ShortLinks.Presentation.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(option => {
    option.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddOptions<CredentialsOptions>()
    .BindConfiguration("Credentials");

builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapUsersRoutes();

app.MapShortLinksRoutes();


app.Run();
