using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StaffWork.Api.Controllers;
using StaffWork.Core.Consts;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using StaffWork.Core.Paramaters;
using StaffWork.Infrastructure.Filters;

namespace StaffWork.Web.Controllers
{
    public class DepartmentController : ApiBaseController<Department>
    {
        public readonly IServicesBase<Administration> AdministrationService;

        public DepartmentController(IServicesBase<Department> servicesBase, IMapper mapper, IServicesBase<Administration> administrationService)
            : base(servicesBase, mapper)
        {
            AdministrationService = administrationService;
        }
        private string GetAuthenticatedUser()
        {
            var userUidClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userUidClaim?.Value!;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await BussinesService.GetAllAsync(null!, ["Administration", "DepartmentAdmin.Admin"]);
            var viewModel = _mapper.Map<IEnumerable<DepartmentViewModel>>(model);

            var model1 = new Tuple<IEnumerable<DepartmentViewModel>, DepartmentFormViewModel>(
                            viewModel, await PopulateViewModelAsync());

            return View(model1);
        }
        [HttpPost]
        public async Task<IActionResult> Create(DepartmentFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var Department = _mapper.Map<Department>(viewModel);
            await BussinesService.InsertAsync(Department);
            return RedirectToAction(nameof(Index), _mapper.Map<DepartmentViewModel>(Department));
        }
        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> EditAsync(int id)
        {
            var Department = await BussinesService.GetAsync(d => d.Id == id);
            if (Department == null)
                return NotFound();
            var viewModel = _mapper.Map<DepartmentFormViewModel>(Department);
            return PartialView("_FormEdit", await PopulateViewModelAsync(viewModel));
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> PostEditAsync(DepartmentFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var department = await BussinesService.GetAsync(d => d.Id == viewModel.Id);
            if (department == null)
                return NotFound();
            _mapper.Map(viewModel, department);

            await BussinesService.UpdateAsync(department.Id, department);
            return RedirectToAction(nameof(Index), _mapper.Map<DepartmentViewModel>(department));
        }
        public async Task<IActionResult> Delete(int id)
        {
            var Department = await BussinesService.GetAsync(d => d.Id == id);
            if (Department == null)
                return NotFound();
            try
            {
                await BussinesService.DeleteAsync(id);
            }
            catch (Exception)
            {
                throw;
            }
            return Ok();
        }
        public async Task<IActionResult> AllowItem(DepartmentViewModel model)
        {
            var Department = await BussinesService.GetAsync(d => d.Name == model.Name);

            var isAllowed = Department is null || Department.Id.Equals(model.Id);

            return Json(isAllowed);
        }

        private async Task<DepartmentFormViewModel> PopulateViewModelAsync(DepartmentFormViewModel? model = null)
        {
            var userId = GetAuthenticatedUser();
            DepartmentFormViewModel viewModel = model is null ? new DepartmentFormViewModel() : model;

            IEnumerable<Administration> administrations;
            if (User.IsInRole(AppRoles.SuperAdmin))
                administrations = await AdministrationService.GetAllAsync();
            else
                administrations = await AdministrationService.GetAllAsync(a => a.ManagerId == userId);

            viewModel.Administrations = _mapper.Map<IEnumerable<SelectListItem>>(administrations);

            return viewModel;
        }
    }
}
