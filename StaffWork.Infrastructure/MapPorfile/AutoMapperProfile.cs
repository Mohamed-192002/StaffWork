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
            CreateMap<Department, SelectListItem>()
               .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Name));

        }
    }
}
