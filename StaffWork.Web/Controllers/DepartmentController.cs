using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StaffWork.Api.Controllers;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using StaffWork.Core.Paramaters;
using StaffWork.Infrastructure.Filters;

namespace StaffWork.Web.Controllers
{
	public class DepartmentController : ApiBaseController<Department>
	{
		public DepartmentController(IServicesBase<Department> servicesBase, IMapper mapper)
			: base(servicesBase, mapper)
		{
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var model = await BussinesService.GetAllAsync();
			var viewModel = _mapper.Map<IEnumerable<DepartmentViewModel>>(model);

            var model1 = new Tuple<IEnumerable<DepartmentViewModel>, DepartmentViewModel>(
                            viewModel, new DepartmentViewModel());

            return View(model1);
		}
		[HttpPost]
		public async Task<IActionResult> Create(DepartmentViewModel viewModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			var Department = _mapper.Map<Department>(viewModel);
			await BussinesService.InsertAsync(Department);
		   return RedirectToAction("Index", _mapper.Map<DepartmentViewModel>(Department));
		}
		[HttpGet]
		[AjaxOnly]
		public async Task<IActionResult> EditAsync(int id)
		{
			var Department = await BussinesService.GetAsync(d => d.Id == id);
			if (Department == null)
				return NotFound();
			var viewModel = _mapper.Map<DepartmentViewModel>(Department);
			return PartialView("_FormEdit", viewModel);
		}
		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> EditAsync(DepartmentViewModel viewModel)
		{
			if (!ModelState.IsValid)
				return BadRequest();
            var department = await BussinesService.GetAsync(d => d.Id == viewModel.Id);
            if (department == null)
                return NotFound();
            _mapper.Map(viewModel, department);

            await BussinesService.UpdateAsync(department.Id, department);
			return RedirectToAction("Index", _mapper.Map<DepartmentViewModel>(department));
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
	}
}
