using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using ShortLinks.Application.Services;
using ShortLinks.Presentation.Api.Dto;

namespace ShortLinks.Presentation.Api;

public static class UsersRoutes {
    public static void MapUsersRoutes(this WebApplication application) {
        
        
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
        
        application.MapPost("login", [AllowAnonymous] async (LoginRequestDto model, IUsersService service, CancellationToken c) => {
            var loginResponse = await service.LoginAsync(
                new UserInfo { Password = model.Password, Username = model.UserName, }, c
            );

            return loginResponse switch {
                { IsSuccess: false } => Results.Unauthorized(),
                { Value: var r } => Results.Ok(new { r.User.UserName, Token = r.Token })
            };
        });

    }
}