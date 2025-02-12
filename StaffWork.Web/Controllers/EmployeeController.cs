using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StaffWork.Api.Controllers;
using StaffWork.Core.Consts;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using StaffWork.Core.Paramaters;
using StaffWork.Infrastructure.Filters;
using StaffWork.Infrastructure.Implementations;
using System.Security.Claims;

namespace StaffWork.Web.Controllers
{
    public class EmployeeController : ApiBaseController<Employee>
    {
        public readonly IServicesBase<User> UserService;

        public EmployeeController(IServicesBase<Employee> servicesBase, IMapper mapper, IServicesBase<User> userService)
            : base(servicesBase, mapper)
        {
            UserService = userService;
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

        [HttpPost]
        public async Task<IActionResult> GetEmployees()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var user = await UserService.GetAsync(x => x.Id == userId);

            var skip = int.Parse(Request.Form["start"]!);
            var pageSize = int.Parse(Request.Form["length"]!);
            var searchValue = Request.Form["search[value]"];
            var sortColumnIndex = Request.Form["order[0][column]"];
            var sortColumn = Request.Form[$"columns[{sortColumnIndex}][name]"];
            var sortColumnDirection = Request.Form["order[0][dir]"];

            IQueryable<Employee> EmployeeQuery;
            EmployeeQuery = (IQueryable<Employee>)await BussinesService.GetAllAsync(null!);

            if (!string.IsNullOrEmpty(searchValue))
            {
                EmployeeQuery = EmployeeQuery.Where(b => b.FullName.Contains(searchValue!)
                || (b.Court == null || b.Court.Contains(searchValue!))
                || (b.Appeal == null || b.Appeal.Contains(searchValue!)));
            }

            var Employee = EmployeeQuery.ToList();

            // Apply sorting in-memory based on known property names
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                switch (sortColumn)
                {
                    case "FullName":
                        Employee = sortColumnDirection == "asc" ? Employee.OrderBy(b => b.FullName).ToList() : Employee.OrderByDescending(b => b.FullName).ToList();
                        break;
                    case "Court":
                        Employee = sortColumnDirection == "asc" ? Employee.OrderBy(b => b.Court).ToList() : Employee.OrderByDescending(b => b.Court).ToList();
                        break;
                    case "Appeal":
                        Employee = sortColumnDirection == "asc" ? Employee.OrderBy(b => b.Appeal).ToList() : Employee.OrderByDescending(b => b.Appeal).ToList();
                        break;
                    default:
                        Employee = Employee.OrderByDescending(b => b.DateCreated).ToList(); // Default sorting
                        break;
                }
            }
            var recordsTotal = Employee.Count;
            Employee = Employee.ToList();
            var data = Employee.Skip(skip).Take(pageSize).ToList();

            var mappedData = _mapper.Map<IEnumerable<EmployeeViewModel>>(Employee);
          
            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = mappedData };

            return Ok(jsonData);
        }
    }
}
