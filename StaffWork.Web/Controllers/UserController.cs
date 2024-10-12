using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StaffWork.Api.Controllers;
using StaffWork.Core.Consts;
using StaffWork.Core.Data;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using StaffWork.Core.Paramaters;
using StaffWork.Infrastructure.Filters;
using StaffWork.Infrastructure.Implementations;
using System.Security.Claims;

namespace StaffWork.Web.Controllers
{
    public class UserController : ApiBaseController<User>
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public readonly IServicesBase<Department> DeptService;

        public UserController(IServicesBase<User> servicesBase, IMapper mapper, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IServicesBase<Department> deptService)
            : base(servicesBase, mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            DeptService = deptService;
        }
        private string GetAuthenticatedUser()
        {
            var userUidClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userUidClaim?.Value!;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var user = await BussinesService.GetAsync(x => x.Id == userId, ["Department"]);
            var IsSuperAdmin = User.IsInRole(AppRoles.SuperAdmin);
            IEnumerable<UserViewModel> viewModels;
            if (IsSuperAdmin)
                viewModels = _mapper.Map<IEnumerable<UserViewModel>>(await BussinesService.GetAllAsync(null!, ["Department"]));
            else
                viewModels = _mapper.Map<IEnumerable<UserViewModel>>(await BussinesService.GetAllAsync(d => d.DepartmentId == user.DepartmentId, ["Department"]));


            var model = new Tuple<IEnumerable<UserViewModel>, UserFormViewModel>(
                            viewModels, await PopulateViewModelAsync());

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            User user = new()
            {
                FullName = model.FullName,
                UserName = model.UserName,
                Password = model.Password!,
                IsActive = true,
                DateCreated = DateTime.Now,
                DepartmentId = model.DepartmentId,
            };

            var result = await _userManager.CreateAsync(user, model.Password!);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.SelectedRole);

                var viewModel = _mapper.Map<UserViewModel>(user);
                return RedirectToAction("Index");
            }

            return BadRequest(string.Join(',', result.Errors.Select(e => e.Description)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatusAsync(string id)
        {
            var user = await BussinesService.GetAsync(d => d.Id == id);

            if (user is null)
                return NotFound();

            user.IsActive = !user.IsActive;

            await _userManager.UpdateAsync(user);

            return Ok();
        }

        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> EditAsync(string id)
        {
            var user = await BussinesService.GetAsync(d => d.Id == id, ["Department"]);
            if (user == null)
                return NotFound();
            var model = _mapper.Map<UserFormViewModel>(user);
            var viewModel = await PopulateViewModelAsync(model);

            return PartialView("_FormEdit", viewModel);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditAsync(UserFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var user = await BussinesService.GetAsync(d => d.Id == viewModel.Id);
            if (user == null)
                return NotFound();
            _mapper.Map(viewModel, user);

            await _userManager.UpdateAsync(user);
            return RedirectToAction("Index", _mapper.Map<UserViewModel>(user));
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await BussinesService.GetAsync(d => d.Id == id);
            if (user == null)
                return NotFound();
            try
            {
                await _userManager.DeleteAsync(user);
            }
            catch (Exception)
            {
                throw;
            }
            return Ok();
        }

        public async Task<IActionResult> AllowUserName(UserFormViewModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            var isAllowed = user is null || user.Id.Equals(model.Id);

            return Json(isAllowed);
        }

        private async Task<UserFormViewModel> PopulateViewModelAsync(UserFormViewModel? model = null)
        {
            var userId = GetAuthenticatedUser();
            UserFormViewModel viewModel = model is null ? new UserFormViewModel() : model;

            IEnumerable<Department> departments;
            if (User.IsInRole(AppRoles.SuperAdmin))
                departments = await DeptService.GetAllAsync();
            else
                departments = await DeptService.GetAllAsync(d => d.Users.Any(u => u.Id == userId));

            viewModel.Departments = _mapper.Map<IEnumerable<SelectListItem>>(departments);

            List<IdentityRole> roles;
            if (User.IsInRole(AppRoles.SuperAdmin))
                roles = [.. _roleManager.Roles
                  .Where(r => r.Name!=AppRoles.SuperAdmin)
                   .OrderBy(a => a.Name)];
            else
                roles = [.. _roleManager.Roles
                  .Where(r => r.Name!=AppRoles.SuperAdmin&&r.Name!=AppRoles.Admin)
                  .OrderBy(a => a.Name)];
            viewModel.Roles = roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name
            }).ToList();

            return viewModel;
        }
    }
}
