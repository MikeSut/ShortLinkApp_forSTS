using app_shortlink.DAL;
using app_shortlink.DAL.Repository;
using app_shortlink.DAL.Repository.IRepository;
using app_shortlink.Domain.Dto;
using Microsoft.EntityFrameworkCore;
using app_shortlink.Domain.Entity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(option =>
{
    option.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapPost("/shortlink", async (UrlRequestDto url, ApplicationDbContext db, HttpContext ctx) =>
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
    db.SaveChangesAsync();

    var result = $"{ctx.Request.Scheme}://{ctx.Request.Host}/{sUrl.ShortUrl}";

    return Results.Ok(new UrlResponseDto()
    {
        ShortUrl = result 
    });

});

app.MapFallback(async (ApplicationDbContext db, HttpContext ctx) =>
{
    var path = ctx.Request.Path.ToUriComponent().Trim('/');
    var urlMatch = await db.TableUrls.FirstOrDefaultAsync(x => 
        x.ShortUrl.ToLower().Trim() == path.ToLower().Trim());
    
    if (urlMatch == null)
        return Results.BadRequest("Invalid request");

    return Results.Redirect(urlMatch.FullUrl);
});

 
app.Run();

