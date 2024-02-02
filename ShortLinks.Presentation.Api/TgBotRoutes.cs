using Microsoft.AspNetCore.Authorization;
using ShortLinks.Application;
using ShortLinks.Application.Services;
using ShortLinks.Presentation.Api.Dto;

namespace ShortLinks.Presentation.Api;

public static class TgBotRoutes
{
    public static void MapTgBotRoutes(this WebApplication app)
    {
        app.MapGet("getTgBot", [Authorize] (HttpContext context,ApplicationDbContext db) =>
        {
            var registeredUser = new RegisteredUser(db, context);
            if (!registeredUser.IsRegUser())
            {
                return Results.Unauthorized();
            }
             
            return Results.Ok(new TgBotResponse()
            {
                Link = $"t.me/csharp_ShortLink_bot?start={registeredUser.CurrentUserName()}"
            });
        });
    }
}