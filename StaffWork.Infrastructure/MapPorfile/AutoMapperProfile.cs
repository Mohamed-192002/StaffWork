using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using StaffWork.Core.Consts;
using StaffWork.Core.Models;
using StaffWork.Core.Paramaters;

namespace StaffWork.Infrastructure
{
    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile()
        {
            CreateMap<WorkDaily, WorkDailyViewModel>()
               .ForMember(dest => dest.DeptName, opt => opt.MapFrom(src => src.User!.Department!.Name))
               .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User!.FullName))
               .ForMember(dest => dest.WorkType, opt => opt.MapFrom(src => src.WorkType!.Name))
               .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Enum.Parse<Status>(src.Status.ToString()))) // Convert String to Enum
                .ReverseMap();
            CreateMap<WorkDaily, WorkDailyFormViewModel>().ReverseMap();
            CreateMap<WorkDaily, WorkDailyEditFormViewModel>().ReverseMap();
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
            CreateMap<User, UpdateUserFormViewModel>().ReverseMap();

        }
    }
}
