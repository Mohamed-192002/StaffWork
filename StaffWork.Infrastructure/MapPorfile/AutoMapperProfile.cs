using System.Security.Principal;
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
            CreateMap<User, SelectListItem>()
           .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.FullName));


            CreateMap<Notification, NotificationViewModel>()
               .ReverseMap();

            CreateMap<TaskModel, TaskModelViewModel>()
               .ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.AssignedUsers!.Select(x => x.User.FullName)))
               .ReverseMap();
            CreateMap<TaskModel, TaskModelFormViewModel>()
               .ForMember(dest => dest.TaskFiles, opt => opt.Ignore());
            CreateMap<TaskModelFormViewModel, TaskModel>()
             .ForMember(dest => dest.TaskFiles, opt => opt.Ignore());

            CreateMap<TaskFile, TaskFileDisplay>()
               .ReverseMap();

            CreateMap<TaskReminder, TaskReminderViewModel>()
               .ForMember(dest => dest.TaskModelTitle, opt => opt.MapFrom(src => src.TaskModel.Title))
               .ForMember(dest => dest.CreatedByUserName, opt => opt.MapFrom(src => src.CreatedByUser.FullName))
               .ReverseMap();

        }
    }
}
