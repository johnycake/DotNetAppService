using AutoMapper;
using DotNetCoreApp1.Controllers.Types;
using DotNetCoreApp1.Models.Types;

namespace DotNetCoreApp1.AutoMapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom( src => Guid.NewGuid()));
            CreateMap<Tuple<User, UserDto>, PasswordDto>()
                .ForMember(dest => dest.PasswordId, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.PasswordValue, opt => opt.MapFrom(src => src.Item1.Password))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Item2.UserId));
            CreateMap<PasswordChange, PasswordDto>()
                .ForMember(x => x.PasswordId, opt =>
                {
                    opt.UseDestinationValue();
                    opt.Ignore();
                })
                .ForMember(x => x.UserId, opt =>
                {
                    opt.UseDestinationValue();
                    opt.Ignore();
                })
                .ForMember(dest => dest.PasswordValue, opt => opt.MapFrom(src => src.NewPassword));
        }
    }
}
