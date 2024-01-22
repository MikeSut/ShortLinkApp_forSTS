using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using PRTelegramBot.Core;
using ShortLinks.Application;
using ShortLinks.Presentation.Api.Dto;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShortLinks.Presentation.Api;

public static class TgRoutes
{
    public static void MapTgRoutes(this WebApplication app)
    {
        app.MapGet("ConnectTelegram", async (
            HttpContext ctx, ApplicationDbContext db) =>
        {
            int currentUserId = int.Parse(ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var AnonUserId = db.Users.First(x => x.UserName == "anonymous").Id;
            if (currentUserId == AnonUserId)
            {
                return Results.Unauthorized();
            }

            
            return Results.Redirect("");



        });
    }
    
   


}