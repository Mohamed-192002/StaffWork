using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StaffWork.Api.Controllers;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using StaffWork.Core.Paramaters;
using StaffWork.Infrastructure.Filters;

namespace StaffWork.Web.Controllers
{
    public class WorkTypeController : ApiBaseController<WorkType>
    {
        public WorkTypeController(IServicesBase<WorkType> servicesBase, IMapper mapper) : base(servicesBase, mapper)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await BussinesService.GetAllAsync();
            var viewModel = _mapper.Map<IEnumerable<WorkTypeViewModel>>(model);

            var model1 = new Tuple<IEnumerable<WorkTypeViewModel>, WorkTypeViewModel>(
                            viewModel, new WorkTypeViewModel());

            return View(model1);
        }
        [HttpPost]
        public async Task<IActionResult> Create(WorkTypeViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var WorkType = _mapper.Map<WorkType>(viewModel);
            await BussinesService.InsertAsync(WorkType);
            return RedirectToAction("Index", _mapper.Map<WorkTypeViewModel>(WorkType));
        }
        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> EditAsync(int id)
        {
            var WorkType = await BussinesService.GetAsync(d => d.Id == id);
            if (WorkType == null)
                return NotFound();
            var viewModel = _mapper.Map<WorkTypeViewModel>(WorkType);
            return PartialView("_FormEdit", viewModel);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditAsync(WorkTypeViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var WorkType = await BussinesService.GetAsync(d => d.Id == viewModel.Id);
            if (WorkType == null)
                return NotFound();
            _mapper.Map(viewModel, WorkType);

            await BussinesService.UpdateAsync(WorkType.Id, WorkType);
            return RedirectToAction("Index", _mapper.Map<WorkTypeViewModel>(WorkType));
        }
        public async Task<IActionResult> Delete(int id)
        {
            var WorkType = await BussinesService.GetAsync(d => d.Id == id);
            if (WorkType == null)
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
        public async Task<IActionResult> AllowItem(WorkTypeViewModel model)
        {
            var WorkType = await BussinesService.GetAsync(d => d.Name == model.Name);

            var isAllowed = WorkType is null || WorkType.Id.Equals(model.Id);

            return Json(isAllowed);
        }
    }
}
