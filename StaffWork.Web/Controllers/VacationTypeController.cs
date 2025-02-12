using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StaffWork.Api.Controllers;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using StaffWork.Core.Paramaters;
using StaffWork.Infrastructure.Filters;

namespace StaffWork.Web.Controllers
{
	public class VacationTypeController : ApiBaseController<VacationType>
	{
		public VacationTypeController(IServicesBase<VacationType> servicesBase, IMapper mapper)
			: base(servicesBase, mapper)
		{
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var model = await BussinesService.GetAllAsync();
			var viewModel = _mapper.Map<IEnumerable<VacationTypeViewModel>>(model);

            var model1 = new Tuple<IEnumerable<VacationTypeViewModel>, VacationTypeViewModel>(
                            viewModel, new VacationTypeViewModel());

            return View(model1);
		}
		[HttpPost]
		public async Task<IActionResult> Create(VacationTypeViewModel viewModel)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			var VacationType = _mapper.Map<VacationType>(viewModel);
			await BussinesService.InsertAsync(VacationType);
		   return RedirectToAction("Index", _mapper.Map<VacationTypeViewModel>(VacationType));
		}
		[HttpGet]
		[AjaxOnly]
		public async Task<IActionResult> EditAsync(int id)
		{
			var VacationType = await BussinesService.GetAsync(d => d.Id == id);
			if (VacationType == null)
				return NotFound();
			var viewModel = _mapper.Map<VacationTypeViewModel>(VacationType);
			return PartialView("_FormEdit", viewModel);
		}
		[HttpPost]
		[AutoValidateAntiforgeryToken]
		public async Task<IActionResult> EditAsync(VacationTypeViewModel viewModel)
		{
			if (!ModelState.IsValid)
				return BadRequest();
            var VacationType = await BussinesService.GetAsync(d => d.Id == viewModel.Id);
            if (VacationType == null)
                return NotFound();
            _mapper.Map(viewModel, VacationType);

            await BussinesService.UpdateAsync(VacationType.Id, VacationType);
			return RedirectToAction("Index", _mapper.Map<VacationTypeViewModel>(VacationType));
		}
		public async Task<IActionResult> Delete(int id)
		{
			var VacationType = await BussinesService.GetAsync(d => d.Id == id);
			if (VacationType == null)
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
		public async Task<IActionResult> AllowItem(VacationTypeViewModel model)
		{
			var VacationType = await BussinesService.GetAsync(d => d.Name == model.Name);

			var isAllowed = VacationType is null || VacationType.Id.Equals(model.Id);

			return Json(isAllowed);
		}
	}
}
