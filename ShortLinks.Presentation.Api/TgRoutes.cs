using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ShortLinks.Application;
using ShortLinks.Presentation.Api.Dto;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ShortLinks.Presentation.Api;

public static class TgRoutes
{
    public static void MapTgRoutes(this WebApplication app)
    {
        app.MapGet("ConnectTelegram", [Authorize] async (TgRequestDto tgRequest, 
            HttpContext ctx, ApplicationDbContext db) =>
        {
            int currentUserId = int.Parse(ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var AnonUserId = db.Users.First(x => x.UserName == "anonymous").Id;
            if (currentUserId == AnonUserId)
            {
                return Results.Unauthorized();
            }
            var client = new TelegramBotClient("");
            client.StartReceiving(Update, Error);
            

            return Results.Ok("all ok");


        });
    }
    
    private static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
    {
        var message = update.Message;
        Console.WriteLine($"{message.Chat.FirstName}  |  {message.Text}");
        if (message.Text != null)
        {
            if (message.Text.ToLower().Contains("здорова"))
            {
                botClient.SendTextMessageAsync(message.Chat.Id, "Изенер");
                
            }
        }

        return botClient.SendTextMessageAsync(message.Chat.Id, "empty");
        

    }
    
    private static Task Error(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
    {
        throw new NotImplementedException();
    }


}