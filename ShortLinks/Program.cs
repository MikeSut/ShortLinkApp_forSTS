using ShortLinks.Application;
using Microsoft.EntityFrameworkCore;
using ShortLinks.Application.Services;
using ShortLinks.Configurations;
using ShortLinks.Presentation.Api;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(option => {
    option.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = "localhost";
    options.InstanceName = "local";
});

builder.Services.AddOptions<CredentialsOptions>()
    .BindConfiguration("Credentials");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IUsersService, UsersService>();

builder.Services.ConfigureSwagger();
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.MapUsersRoutes();
app.MapShortLinksRoutes();

app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();


app.Run();
