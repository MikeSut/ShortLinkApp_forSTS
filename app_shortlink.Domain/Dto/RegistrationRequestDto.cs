using app_shortlink.Domain.Enum;

namespace app_shortlink.Domain.Dto;

public class RegistrationRequestDto
{
    public string Name { get; set; }
      
    public string UserName { get; set; }
    
    public string Password { get; set; }
    
    public string Role { get; set; }
}