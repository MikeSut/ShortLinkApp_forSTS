using System.Runtime.CompilerServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using ShortLinks.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using ShortLinks.Application;
using ShortLinks.Presentation.Api.Dto;

namespace ShortLinks.Presentation.Api;

public class UrlRequestDto()
{
    public string FullUrl { get; set; } = "";
}

public static class ShortLinksRoutes {
    
   
    
    public static void MapShortLinksRoutes(this WebApplication application) {
        
        application.MapPost("/shortlink", [Authorize] async (UrlRequestDto url, ApplicationDbContext db, HttpContext ctx) =>
        {
            
            int currentUserId = int.Parse(ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var AnonUser = db.Users.First(x => x.UserName == "anonymous");

        
            //Проверяем входной url
            if (!Uri.TryCreate(url.FullUrl, UriKind.Absolute, out var inputUrl))
                return Results.BadRequest("Был предоставлен неверный Url-адрес");

            //Проверяем есть ли входной url в Urls и был ли он уже преобразован в короткую ссылку для текущего пользователя
            //А так же не является ли пользователь анонимным
            //Если условие истинно возвращаем уже сгенерированную короткую ссылку и количество переходов по ней
            var strUrl = await db.Urls.FirstOrDefaultAsync(x => 
                x.FullUrl == url.FullUrl & x.UserId == currentUserId & currentUserId != AnonUser.Id);
            
           if (strUrl != null)
            {
                var respUrl = $"{ctx.Request.Scheme}://{ctx.Request.Host}/{strUrl.ShortUrl}";
                var amountClicks = db.IpClients.Where(x => x.UrlId == strUrl.Id);
                
                
                return Results.Ok(new CountUrlResponseDto()
                {
                    ShortUrl = respUrl,
                    AmountClicks = amountClicks.Count()
                });
            }
            
            //Создаем короткую версию предоставленного Url
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@az";
            var randomStr = new string(Enumerable.Repeat(chars, 8)
                .Select(x => x[random.Next(x.Length)]).ToArray());
            var creationDate = DateTime.UtcNow.Date;

            var expirationDate = (currentUserId == AnonUser.Id) ? creationDate.AddDays(4) : creationDate.AddDays(29);
            var sUrl = new Url()
            {
                FullUrl = url.FullUrl,
                ShortUrl = randomStr,
                UserId = currentUserId,
                CreationDate = creationDate,
                ExpirationDate = expirationDate
            };
            db.Urls.Add(sUrl);
            await db.SaveChangesAsync();
            
            var result = $"{ctx.Request.Scheme}://{ctx.Request.Host}/{sUrl.ShortUrl}";
            
            //Вывод для Анонимного пользователя
            if (currentUserId == AnonUser.Id)
            {
                return Results.Ok(new AnonUrlResponseDto()
                {
                    Message = $"Ссылка активна до {expirationDate}",
                    ShortUrl = result
                });
            }
            
            return Results.Ok(new UrlResponseDto()
            {
                Message = $"Ссылка активна до {expirationDate}",
                ShortUrl = result
            });
        });

        application.MapFallback(async (ApplicationDbContext db, HttpContext ctx) =>
        {
            var path = ctx.Request.Path.ToUriComponent().Trim('/');
            var urlMatch = await db.Urls.FirstOrDefaultAsync(x =>
                x.ShortUrl.ToLower().Trim() == path.ToLower().Trim());
            var ClientIpAddress = ctx.Connection.RemoteIpAddress.ToString();
            var IdUrl = await db.Urls.FirstOrDefaultAsync(x => x.ShortUrl == path);

            if (urlMatch == null)
                return Results.BadRequest("Invalid request");

            var sIpClient = new IpClient()
            {
                ClientIP = ClientIpAddress,
                UrlId = IdUrl.Id
            };
            db.IpClients.Add(sIpClient);
            await db.SaveChangesAsync();
            
            
            return Results.Redirect(urlMatch.FullUrl);
        });
    }
}