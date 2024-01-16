using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ShortLinks.Application;
using ShortLinks.Domain.Entity;
using ShortLinks.Presentation.Api.Dto;

namespace ShortLinks.Presentation.Api;

public static class TgRoutes
{
    public static void MapTgRoutes(this WebApplication app)
    {
        app.MapPost("ConnectTelegram", [Authorize] async (TgRequestDto tgRequest, 
            HttpContext ctx, ApplicationDbContext db) =>
        {
            int currentUserId = int.Parse(ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var AnonUserId = db.Users.First(x => x.UserName == "anonymous").Id;
            if (currentUserId == AnonUserId)
            {
                return Results.Unauthorized();
            }

            var sPhoneNumber = new PhoneNumber()
            {
                UserId = currentUserId,
                Phone = tgRequest.PhoneNumber
            };
            db.PhoneNumbers.Add(sPhoneNumber);
            await db.SaveChangesAsync();

            return Results.Ok("all ok");


        });
    }
}