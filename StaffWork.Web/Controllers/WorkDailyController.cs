using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StaffWork.Api.Controllers;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using StaffWork.Core.Paramaters;
using StaffWork.Infrastructure.Implementations;
using System.Security.Claims;

namespace StaffWork.Web.Controllers
{
    public class WorkDailyController : ApiBaseController<WorkDaily>
    {
        public readonly IServicesBase<User> UserService;
        public readonly IServicesBase<Department> DepartmentService;
        public readonly IServicesBase<WorkType> WorkTypesService;
        public WorkDailyController(IServicesBase<WorkDaily> servicesBase, IMapper mapper, IServicesBase<User> userService, IServicesBase<Department> departmentService, IServicesBase<WorkType> workTypesService)
            : base(servicesBase, mapper)
        {
            UserService = userService;
            DepartmentService = departmentService;
            WorkTypesService = workTypesService;
        }
        public async Task<IActionResult> IndexDateAsync()
        {
            var model = await BussinesService.GetAllAsync(null!, ["User", "User.Department"]);
            var viewModel = _mapper.Map<IEnumerable<WorkDailyViewModel>>(model);

            var model1 = new Tuple<IEnumerable<WorkDailyViewModel>, DateViewModel>(
                            viewModel, new DateViewModel());

            return View(model1);
        }
        public async Task<IActionResult> IndexAsync(DateViewModel date)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            var user = await UserService.GetAsync(u => u.Id == userId, ["WorkDailies", "Department"]);
            var workDailies = new List<WorkDaily>();
            if (user.WorkDailies != null && user.WorkDailies.Any())
            {
                workDailies = user.WorkDailies.ToList();
            }
            var workTypes = _mapper.Map<IEnumerable<SelectListItem>>(await WorkTypesService.GetAllAsync());

            var model = new Tuple<User, IList<WorkDaily>, IEnumerable<SelectListItem>>(user, workDailies, workTypes);

            return View("Index", model);
        }


    }
}
