using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ShortLinks.Application.Services;

public class RegisteredUser(ApplicationDbContext db, HttpContext context)
{
    private readonly int _currentUserId = int.Parse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
    private readonly int _anonUserId = db.Users.First(x => x.UserName == "anonymous").Id;
    public bool IsRegUser()
    {
        return _currentUserId != _anonUserId;
    }

    public string CurrentUserName()
    {
        return db.Users.First(x => x.Id == _currentUserId).UserName;
    }
}