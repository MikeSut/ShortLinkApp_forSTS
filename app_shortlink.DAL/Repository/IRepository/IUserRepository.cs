using app_shortlink.Domain.Dto;
using app_shortlink.Domain.Entity;

namespace app_shortlink.DAL.Repository.IRepository;

public interface IUserRepository
{
    bool IsUniqueUser(string username);

    Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
    
    Task<User> Register(RegistrationRequestDto registrationRequestDto);

}