using Microsoft.AspNetCore.Authorization;
using ShortLinks.Application;
using ShortLinks.Application.Services;
using ShortLinks.Presentation.Api.Dto;

namespace ShortLinks.Presentation.Api;

public static class UsersRoutes {
    public static void MapUsersRoutes(this WebApplication application)
    {

        application.MapGet("anonymous", async (HttpContext context,ApplicationDbContext db, IUsersService service, CancellationToken c) =>
        {
            var user = db.Users.FirstOrDefault(x => x.UserName == "anonymous");
            var loginResponse = await service.LoginAsync(
                new UserInfo { Password = user.Password, Username = user.UserName, }, context, c);
            
            return loginResponse switch {
                { Value: var r } => Results.Ok(new { Token = r.Token })
            };
        });
        
        
        application.MapPost("register",  async (UserRegistration userRegistration, IUsersService service) =>
        {
            
            bool ifUserNameUnique = service.IsUsernameTaken(userRegistration.UserName);
            if (ifUserNameUnique)
            {
                return Results.BadRequest("Username already exists");
            }

            var user = await service.Register(userRegistration);
            if (user == null)
            {
                return Results.BadRequest("Error while register");
            }
            

            return Results.Ok("Регистрация прошла успешно!");
        });
        
        application.MapPost("login", [AllowAnonymous] async (HttpContext context, LoginRequestDto model, IUsersService service, CancellationToken c) => {
            var loginResponse = await service.LoginAsync(
                new UserInfo { Password = model.Password, Username = model.UserName, }, context, c
            );
            return loginResponse switch {
                { IsSuccess: false } => Results.Unauthorized(),
                { Value: var r } => Results.Ok(new { r.User.UserName, r.Token, r.YourShortLinks })
            };
        });

    }
}