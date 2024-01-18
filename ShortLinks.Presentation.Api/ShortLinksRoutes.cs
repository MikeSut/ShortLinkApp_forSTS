using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using ShortLinks.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using ShortLinks.Application;
using ShortLinks.Application.Services;
using ShortLinks.Presentation.Api.Dto;

namespace ShortLinks.Presentation.Api;


public static class ShortLinksRoutes {
    
   
    
    public static void MapShortLinksRoutes(this WebApplication application) {
        
        application.MapPost("/shortlink", [Authorize] async (UrlRequestDto url, ApplicationDbContext db, HttpContext ctx, CacheService cacheService) =>
        {
            //Проверяем входной url
            if (!Uri.TryCreate(url.FullUrl, UriKind.Absolute, out var inputUrl))
                return Results.BadRequest("Был предоставлен неверный Url-адрес");
            
            int currentUserId = int.Parse(ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var AnonUser = db.Users.First(x => x.UserName == "anonymous"); 
            
        
            

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
 

            var lifeTime = (currentUserId == AnonUser.Id) ? 4 : ((url.LifeTimeLink != 0) ? url.LifeTimeLink - 1 : 9);
            if (lifeTime < 1 | lifeTime > 30)
            {
                return Results.BadRequest("Возможна установка срока действия ссылки от 1 до 30 дней.");
            }

            if (url.Permanent != "string" & currentUserId != AnonUser.Id)
            {
                if (url.Permanent.ToLower() != "yes" & url.Permanent.ToLower() != "no")
                {
                    return Results.BadRequest("Поле 'Permanent' может принимать только параметры 'Yes/No'.");
                }
            }
            var permanent = (currentUserId == AnonUser.Id) ? "no" : ((url.Permanent.ToLower() == "yes") ? "yes" : "no");

           
            
           
            
            //Создаем короткую версию предоставленного Url
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@az";
            var randomStr = new string(Enumerable.Repeat(chars, 8)
                .Select(x => x[random.Next(x.Length)]).ToArray());
            
            var creationDate = DateTime.UtcNow.Date;
            var expirationDate = creationDate.AddDays(lifeTime);
            
            var sUrl = new Url()
            {
                FullUrl = url.FullUrl,
                ShortUrl = randomStr,
                UserId = currentUserId,
                ExpirationDate = expirationDate,
                Permanent = permanent
            };
            await cacheService.AddUrl(sUrl);

            if (currentUserId != AnonUser.Id)
            {
                db.Urls.Add(sUrl);
                await db.SaveChangesAsync();
            }
            var result = $"{ctx.Request.Scheme}://{ctx.Request.Host}/{sUrl.ShortUrl}";
            
            //Вывод для Анонимного пользователя
            if (currentUserId == AnonUser.Id)
            {
                return Results.Ok(new AnonUrlResponseDto()
                {
                    Message = $"Так как вы являетесь анонимным пользователем ваша ссылка будет активна 5 суток, т.е. до {expirationDate}",
                    ShortUrl = result
                });
            }

            if (permanent == "yes")
            {
                return Results.Ok(new UrlResponseDto()
                {
                    Message = $"Ваша ссылка перманентна",
                    ShortUrl = result
                });
            }
            return Results.Ok(new UrlResponseDto()
            {
                Message = $"Ссылка активна до {expirationDate}",
                ShortUrl = result,
                
            });
        });

        application.MapFallback(async (ApplicationDbContext db, HttpContext ctx, CacheService cacheService) =>
        {

            var path = ctx.Request.Path.ToUriComponent().Trim('/');
            var permanentLinks = db.Urls.Where(x => x.Permanent == "yes");
            var permanentLink = await permanentLinks.FirstOrDefaultAsync(x =>
                x.ShortUrl.ToLower().Trim() == path.ToLower().Trim());

            var urlMatchBase = await db.Urls.FirstOrDefaultAsync(x =>
                x.ShortUrl.ToLower().Trim() == path.ToLower().Trim());
            var urlMatchCache = await cacheService.GetUrl(path);
            var clientIpAddress = ctx.Connection.RemoteIpAddress.ToString();
            var idUrl = await db.Urls.FirstOrDefaultAsync(x => x.ShortUrl == path);
            Console.WriteLine(urlMatchCache);
            
            
            if (urlMatchBase == null & urlMatchCache == null & permanentLink == null)
                return Results.BadRequest("Page not found.");

            if (urlMatchBase != null)
            {
                var sIpClient = new IpClient()
                {
                    ClientIP = clientIpAddress,
                    UrlId = idUrl.Id
                };
                db.IpClients.Add(sIpClient);
                await db.SaveChangesAsync();
            }

            if (permanentLink != null) return Results.Redirect(permanentLink.FullUrl);
            return Results.Redirect(urlMatchCache);
        });
    }
}