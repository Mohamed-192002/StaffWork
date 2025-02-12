using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StaffWork.Api.Controllers;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using StaffWork.Core.Paramaters;
using StaffWork.Infrastructure.Filters;

namespace StaffWork.Web.Controllers
{
	public class EmployeeController : ApiBaseController<Employee>
	{
		public EmployeeController(IServicesBase<Employee> servicesBase, IMapper mapper)
			: base(servicesBase, mapper)
		{
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var model = await BussinesService.GetAllAsync();
			var viewModel = _mapper.Map<IEnumerable<EmployeeViewModel>>(model);

            var model1 = new Tuple<IEnumerable<EmployeeViewModel>, EmployeeViewModel>(
                            viewModel, new EmployeeViewModel());

            return View(model1);
		}
		[HttpPost]
		public async Task<IActionResult> Create(EmployeeViewModel viewModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			var Employee = _mapper.Map<Employee>(viewModel);
			await BussinesService.InsertAsync(Employee);
		   return RedirectToAction("Index", _mapper.Map<EmployeeViewModel>(Employee));
		}
		[HttpGet]
		[AjaxOnly]
		public async Task<IActionResult> EditAsync(int id)
		{
			var Employee = await BussinesService.GetAsync(d => d.Id == id);
			if (Employee == null)
				return NotFound();
			var viewModel = _mapper.Map<EmployeeViewModel>(Employee);
			return PartialView("_FormEdit", viewModel);
		}
		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> EditAsync(EmployeeViewModel viewModel)
		{
			if (!ModelState.IsValid)
				return BadRequest();
            var Employee = await BussinesService.GetAsync(d => d.Id == viewModel.Id);
            if (Employee == null)
                return NotFound();
            _mapper.Map(viewModel, Employee);

            await BussinesService.UpdateAsync(Employee.Id, Employee);
			return RedirectToAction("Index", _mapper.Map<EmployeeViewModel>(Employee));
		}
		public async Task<IActionResult> Delete(int id)
		{
			var Employee = await BussinesService.GetAsync(d => d.Id == id);
			if (Employee == null)
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
		public async Task<IActionResult> AllowItem(EmployeeViewModel model)
		{
			var Employee = await BussinesService.GetAsync(d => d.FullName == model.FullName);

			var isAllowed = Employee is null || Employee.Id.Equals(model.Id);

			return Json(isAllowed);
		}
	}
}
