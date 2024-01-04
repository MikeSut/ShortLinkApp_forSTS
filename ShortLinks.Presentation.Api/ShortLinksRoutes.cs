using Microsoft.AspNetCore.Authorization;
using ShortLinks.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using ShortLinks.Application;
using ShortLinks.Presentation.Api.Dto;

namespace ShortLinks.Presentation.Api;

public static class ShortLinksRoutes {
    public static void MapShortLinksRoutes(this WebApplication application) {
       
        application.MapPost("/shortlink", [Authorize] async (UrlRequestDto url, ApplicationDbContext db, HttpContext ctx) =>
        {
            
            //Проверяем входной url
            if (!Uri.TryCreate(url.FullUrl, UriKind.Absolute, out var inputUrl))
                return Results.BadRequest("Был предоставлен неверный Url-адрес");

            //Создаем короткую версию предоставленного Url
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@az";
            var randomStr = new string(Enumerable.Repeat(chars, 8)
                .Select(x => x[random.Next(x.Length)]).ToArray());

            var sUrl = new TableUrl()
            {
                FullUrl = url.FullUrl,
                ShortUrl = randomStr

            };
            db.TableUrls.Add(sUrl);

            await db.SaveChangesAsync();
            
            

            var result = $"{ctx.Request.Scheme}://{ctx.Request.Host}/{sUrl.ShortUrl}";
            

            return Results.Ok(new UrlResponseDto()
            {
                ShortUrl = result,
                
            });
        });

        application.MapFallback(async (ApplicationDbContext db, HttpContext ctx) =>
        {
            var path = ctx.Request.Path.ToUriComponent().Trim('/');
            var urlMatch = await db.TableUrls.FirstOrDefaultAsync(x =>
                x.ShortUrl.ToLower().Trim() == path.ToLower().Trim());

            if (urlMatch == null)
                return Results.BadRequest("Invalid request");

            return Results.Redirect(urlMatch.FullUrl);
        });
    }
}