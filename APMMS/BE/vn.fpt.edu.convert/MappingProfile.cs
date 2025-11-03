using AutoMapper;
using BE.models;
using BE.vn.fpt.edu.DTOs.TypeComponent;
using BE.vn.fpt.edu.DTOs.Component; 

namespace BE.vn.fpt.edu.convert
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // TypeComponent mapping
            CreateMap<TypeComponent, DTOs.TypeComponent.ResponseDto>()
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch != null ? src.Branch.Name : null))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.StatusCodeNavigation != null ? src.StatusCodeNavigation.Name : null));

            CreateMap<DTOs.TypeComponent.RequestDto, TypeComponent>();

            // Component mapping
            CreateMap<Component, DTOs.Component.ResponseDto>()
                .ForMember(dest => dest.TypeComponentName, opt => opt.MapFrom(src => src.TypeComponent != null ? src.TypeComponent.Name : null))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch != null ? src.Branch.Name : null))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.StatusCodeNavigation != null ? src.StatusCodeNavigation.Name : null));

            CreateMap<DTOs.Component.RequestDto, Component>();

            // ServicePackage mapping
            CreateMap<ServicePackage, DTOs.ServicePackage.ResponseDto>()
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch != null ? src.Branch.Name : null))
                .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.StatusCodeNavigation != null ? src.StatusCodeNavigation.Name : null))
                .ForMember(dest => dest.ComponentNames, opt => opt.MapFrom(src => src.Components.Select(c => c.Name).ToList()));

            CreateMap<DTOs.ServicePackage.RequestDto, ServicePackage>()
                .ForMember(dest => dest.Components, opt => opt.Ignore());
        }
    }
}
