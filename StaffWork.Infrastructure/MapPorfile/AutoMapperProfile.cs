using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using StaffWork.Core.Models;
using StaffWork.Core.Paramaters;

namespace StaffWork.Infrastructure
{
    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile()
        {
            CreateMap<WorkType, WorkTypeViewModel>().ReverseMap();
            CreateMap<WorkType, SelectListItem>()
              .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
              .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

            CreateMap<Department, DepartmentViewModel>().ReverseMap();
            CreateMap<Department, SelectListItem>()
               .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

            CreateMap<User, UserViewModel>()
               .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department!.Name))
           .ReverseMap();
            CreateMap<User, UserFormViewModel>().ReverseMap();

        }
    }
}
