namespace ShortLinks.Presentation.Api.Dto;

public class UserDto {

}

public class LoginResponseDto
{
    public UserDto User { get; set; }

    public string Token { get; set; }
}