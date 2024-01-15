using Microsoft.AspNetCore.Http;
using ShortLinks.Domain.Entity;

namespace ShortLinks.Application.Services;

public interface IUsersService {
    bool IsUsernameTaken(string username);

    Task<User> Register(UserRegistration userRegistration);
    Task<Result<Login>> LoginAsync(UserInfo userInfo, HttpContext context,CancellationToken c = default);

    
}