using AutoMapper;
using DotNetCoreApp1.Controllers.Types;
using DotNetCoreApp1.Models.Types;

namespace DotNetCoreApp1.AutoMapperProfiles
{
    public class RecordProfile : Profile
    {
        public RecordProfile()
        {
            CreateMap<Record, RecordDto>()
                .ForMember(dest => dest.RecordId, opt => opt.MapFrom( src => Guid.NewGuid()));
        }
    }
}
