using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using app_shortlink.DAL.Repository.IRepository;
using app_shortlink.Domain.Dto;
using app_shortlink.Domain.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace app_shortlink.DAL.Repository;

public class UserRepository: IUserRepository
{
    private readonly ApplicationDbContext _db;

    private string secretKey;
    
    public UserRepository(ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        secretKey = "PROGRAMMING IS DIFFICULT, BUT VERY INTERESTING";
    }
    
    public bool IsUniqueUser(string username)
    {
        var user = _db.Users.FirstOrDefault(x => x.UserName == username);
        if (user == null)
        {
            return true;
        }
        return false;
    }

    public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
    {
        var user = _db.Users.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower()
                                                 && u.Password == loginRequestDto.Password);
        if (user == null)
        {
            return new LoginResponseDto()
            {
                Token = "", 
                User = null
            };

        }
        //если пользователь был найден генерируем JWT Token

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        LoginResponseDto loginResponseDto = new LoginResponseDto()
        {
            Token = tokenHandler.WriteToken(token),
            User = user
        };
        return loginResponseDto;

    }

    public async Task<User> Register(RegistrationRequestDto registrationRequestDto)
    {
        User user = new User()
        {
            Name = registrationRequestDto.Name,
            UserName = registrationRequestDto.UserName,
            Password = registrationRequestDto.Password,
            Role = registrationRequestDto.Role,
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        user.Password = "";
        return user;
    }

    
}