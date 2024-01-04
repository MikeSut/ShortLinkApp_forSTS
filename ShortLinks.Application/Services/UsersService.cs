using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ShortLinks.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ShortLinks.Application.Services;


public class UserRegistration
{
    public string Name { get; set; }
      
    public string UserName { get; set; }
    
    public string Password { get; set; }
    
}
public class Result<T> {
    public bool IsSuccess { get; init; }

    public string Error { get; init; }

    public T Value { get; init; }
}

public class Login {
    public required string Token { get; init; }

    public required User User { get; init; }
}

public class UserInfo {
    public string Username { get; init; }

    public string Password { get; init; }
}

public class UsersService : IUsersService {
    private readonly ApplicationDbContext _db;

    private readonly string secretKey;
    
    public UsersService(ApplicationDbContext db, IOptions<CredentialsOptions> optProvider)
    {
        _db = db;
        secretKey = optProvider.Value.Secret;
    }
    
    public bool IsUsernameTaken(string username) {
        return _db.Users.Any(x => x.UserName == username);
    }

    public async Task<Result<Login>> LoginAsync(UserInfo login, CancellationToken c) {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == login.Username.ToLower(), c);
        
        if (user == null) {
            return new Result<Login> { IsSuccess = false, Error = "User is not found" };
        }
        // если пользователь был найден генерируем JWT Token

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor {
            Subject = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(5),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new Result<Login> {
            IsSuccess = true,
            Value = new Login { Token = tokenHandler.WriteToken(token), User = user }
        };
    }
    
    public async Task<User> Register(UserRegistration userRegistration)
    {
        User user = new User()
        {
            Name = userRegistration.Name,
            UserName = userRegistration.UserName,
            Password = userRegistration.Password,
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        user.Password = "";
        return user;
    }
}