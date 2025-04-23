using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using StaffWork.Api.Controllers;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using StaffWork.Core.Paramaters;
using StaffWork.Infrastructure.Filters;
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
        public IActionResult Index()
        {
            var model = new EmployeeViewModel();

            return View(model);
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
               // || (b.Court == null || b.Court.Contains(searchValue!))
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

            var mappedData = _mapper.Map<IEnumerable<EmployeeViewModel>>(data);

            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = mappedData };

            return Ok(jsonData);
        }
        public async Task<IActionResult> ExportToExcelAsync(string searchValue, string sortColumn, string sortColumnDirection)
        {
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

            // Fetch filtered and sorted data
            var data = _mapper.Map<List<EmployeeViewModel>>(Employee);

            // Create Excel file
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("سجل الموظفين"); // Arabic for "Date"

                // Manually set the Arabic headers
                worksheet.Cells[1, 1].Value = "اسم الموظف";
                worksheet.Cells[1, 2].Value = "المحكمه";
                worksheet.Cells[1, 3].Value = "الاستئناف";

                // Load data starting from row 2 (after the headers)
                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].FullName;
                    worksheet.Cells[i + 2, 2].Value = data[i].Court;
                    worksheet.Cells[i + 2, 3].Value = data[i].Appeal;

                }

                var stream = new MemoryStream();
                package.SaveAs(stream);

                string excelName = $"سجل الموظفين-{DateTime.Now:dd/MM/yyyy}.xlsx"; // Filename in Arabic

                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

    }
}
