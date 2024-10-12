using Microsoft.AspNetCore.Identity;
using StaffWork.Core;
using StaffWork.Core.Data;
using StaffWork.Core.Models;
using StaffWork.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace StaffWork.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();


            builder.Services.AddControllersWithViews();

            builder.Services.AddInfrastructureDependencies()
                           .AddCoreDependencies()
                           .AddServiceRegisteration(builder.Configuration);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=WorkDaily}/{action=IndexDate}/{id?}");
            app.MapRazorPages();
            app.InfrstructureConfigMiddleware();

            app.Run();
        }
    }
}
