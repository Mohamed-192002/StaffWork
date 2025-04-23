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
            services.AddScoped<IServicesBase<VacationType>, VacationTypeService>();
            services.AddScoped<IServicesBase<WorkType>, WorkTypeService>();
            services.AddScoped<IServicesBase<WorkDaily>, WorkDailyService>();
            services.AddScoped<IServicesBase<User>, UserService>();
            services.AddScoped<IServicesBase<Employee>, EmployeeService>();
            services.AddScoped<IServicesBase<Vacation>, VacationsService>();
            services.AddScoped<IServicesBase<Notification>, NotificationService>();
            services.AddScoped<IServicesBase<TaskModel>, TaskModelService>();
            services.AddScoped<IServicesBase<TaskFile>, TaskFileService>();
            services.AddScoped<IServicesBase<TaskReminder>, TaskReminderService>();
            services.AddScoped<IServicesBase<TaskReminderFile>, TaskReminderFileService>();

            return services;
        }
    }
}
