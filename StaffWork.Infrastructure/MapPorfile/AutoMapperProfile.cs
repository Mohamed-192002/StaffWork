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

            CreateMap<Department, DepartmentViewModel>()
               .ForMember(dest => dest.AdministrationName, opt => opt.MapFrom(src => src.Administration!.Name))
               .ForMember(dest => dest.AdminName, opt => opt.MapFrom(src => src.DepartmentAdmin!.Admin!.FullName))
                .ReverseMap();
            CreateMap<Department, DepartmentFormViewModel>().ReverseMap();
            CreateMap<Department, SelectListItem>()
               .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));
            CreateMap<Administration, AdministrationViewModel>()
               .ForMember(dest => dest.AdminName, opt => opt.MapFrom(src => src.Manager!.FullName))
                .ReverseMap();
            CreateMap<Administration, SelectListItem>()
               .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));
            CreateMap<Employee, EmployeeViewModel>().ReverseMap();
            CreateMap<Employee, SelectListItem>()
               .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.FullName));

            CreateMap<VacationType, VacationTypeViewModel>().ReverseMap();
            CreateMap<VacationType, SelectListItem>()
               .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

            CreateMap<User, UserViewModel>()
               .ForMember(dest => dest.Department, opt => opt.MapFrom(src => src.Department!.Name))
           .ReverseMap();
            CreateMap<User, UserFormViewModel>().ReverseMap();
            CreateMap<User, UpdateUserFormViewModel>().ReverseMap();

            CreateMap<Vacation, VacationViewModel>()
              .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee!.FullName))
              .ForMember(dest => dest.Court, opt => opt.MapFrom(src => src.Employee!.Court))
              .ForMember(dest => dest.Appeal, opt => opt.MapFrom(src => src.Employee!.Appeal))
              .ForMember(dest => dest.VacationType, opt => opt.MapFrom(src => src.VacationType!.Name))
          .ReverseMap();
            CreateMap<Vacation, VacationFormViewModel>()
                .ReverseMap();

            CreateMap<Notification, NotificationViewModel>()
               .ReverseMap();

        }
    }
}
