using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StaffWork.Api.Controllers;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using StaffWork.Core.Paramaters;
using StaffWork.Infrastructure.Filters;

namespace StaffWork.Web.Controllers
{
    public class AdministrationController : ApiBaseController<Administration>
    {
        public AdministrationController(IServicesBase<Administration> servicesBase, IMapper mapper)
            : base(servicesBase, mapper)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await BussinesService.GetAllAsync(null!, ["Manager"]);
            var viewModel = _mapper.Map<IEnumerable<AdministrationViewModel>>(model);

            var model1 = new Tuple<IEnumerable<AdministrationViewModel>, AdministrationViewModel>(
                            viewModel, new AdministrationViewModel());

            return View(model1);
        }
        [HttpPost]
        public async Task<IActionResult> Create(AdministrationViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var Administration = _mapper.Map<Administration>(viewModel);
            await BussinesService.InsertAsync(Administration);
            return RedirectToAction(nameof(Index), _mapper.Map<AdministrationViewModel>(Administration));
        }
        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> EditAsync(int id)
        {
            var Administration = await BussinesService.GetAsync(d => d.Id == id);
            if (Administration == null)
                return NotFound();
            var viewModel = _mapper.Map<AdministrationViewModel>(Administration);
            return PartialView("_FormEdit", viewModel);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditAsync(AdministrationViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var Administration = await BussinesService.GetAsync(d => d.Id == viewModel.Id);
            if (Administration == null)
                return NotFound();
            _mapper.Map(viewModel, Administration);

            await BussinesService.UpdateAsync(Administration.Id, Administration);
            return RedirectToAction("Index", _mapper.Map<AdministrationViewModel>(Administration));
        }
        public async Task<IActionResult> Delete(int id)
        {
            var Administration = await BussinesService.GetAsync(d => d.Id == id);
            if (Administration == null)
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
        public async Task<IActionResult> AllowItem(AdministrationViewModel model)
        {
            var Administration = await BussinesService.GetAsync(d => d.Name == model.Name);

            var isAllowed = Administration is null || Administration.Id.Equals(model.Id);

            return Json(isAllowed);
        }
    }

}
