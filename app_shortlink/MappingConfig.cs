using app_shortlink.Domain.Dto;
using app_shortlink.Domain.Entity;
using AutoMapper;

namespace app_shortlink;

public class MappingConfig: Profile
{
    public MappingConfig()
    {
        CreateMap<RegistrationRequestDto, User>()
            .ForMember(dst => dst.Id, opt => opt.Ignore())
            .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dst => dst.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dst => dst.Password, opt => opt.MapFrom(src => src.Password))
            .ForMember(dst => dst.Role, opt => opt.MapFrom(src => src.Role));

        CreateMap<User, LoginResponseDto>()
            .ForMember(dst => dst.Token, opt => opt.Ignore());




    }
}