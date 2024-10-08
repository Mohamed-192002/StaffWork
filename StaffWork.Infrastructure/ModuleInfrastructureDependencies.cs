using Microsoft.Extensions.DependencyInjection;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using StaffWork.Infrastructure.Implementations;
using System.Reflection;

namespace StaffWork.Infrastructure
{
    public static class ModuleInfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
        {
            //Configuration Of Automapper
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddScoped(typeof(IGenericRepo<>), typeof(GenericRepo<>));
            services.AddScoped<IServicesBase<Department>, DepartmentService>();
            services.AddScoped<IServicesBase<WorkType>, WorkTypeService>();
            //services.AddScoped<IServicesBase<Attendance>, AttendanceService>();
            services.AddScoped<IServicesBase<User>, UserService>();

            return services;
        }
    }
}
