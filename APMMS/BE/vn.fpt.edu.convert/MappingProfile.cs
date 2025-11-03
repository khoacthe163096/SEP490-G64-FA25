using AutoMapper;
using BE.models;
using BE.vn.fpt.edu.DTOs.TypeComponent;

namespace BE.vn.fpt.edu.convert
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<TypeComponent, ResponseDto>()
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch != null ? src.Branch.Name : null))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.StatusCodeNavigation != null ? src.StatusCodeNavigation.Name : null));

            CreateMap<RequestDto, TypeComponent>();
        }
    }
}