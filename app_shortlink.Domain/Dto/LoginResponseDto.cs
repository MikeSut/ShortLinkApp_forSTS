using app_shortlink.Domain.Entity;

namespace app_shortlink.Domain.Dto;

public class LoginResponseDto
{
    public User User { get; set; }
    public string Token { get; set; }
}