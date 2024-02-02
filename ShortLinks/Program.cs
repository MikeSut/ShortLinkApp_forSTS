using ShortLinks.Application;
using Microsoft.EntityFrameworkCore;
using ShortLinks.Application.Services;
using ShortLinks.Configurations;
using ShortLinks.Presentation.Api;
using ShortLinks.Presentation.Api.Telegram;


var builder = WebApplication.CreateBuilder(args);

builder.Logging
    .SetMinimumLevel(LogLevel.Debug)
    .AddConsole();

builder.Services.AddDbContext<ApplicationDbContext>(option => {
    option.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddOptions<CredentialsOptions>()
    .BindConfiguration("Credentials");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IUsersService, UsersService>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost";
    options.InstanceName = "local";
});

builder.Services.AddTransient<CacheService>();

builder.Services.ConfigureSwagger();
builder.Services.ConfigureAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddTelegramBotServices();


var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapUsersRoutes();
app.MapShortLinksRoutes();
app.MapTgBotRoutes();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.Run();
