using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using OfficeOpenXml;
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
    public class VacationController : ApiBaseController<Vacation>
    {
        public readonly IServicesBase<Employee> _EmployeeService;
        public readonly IServicesBase<VacationType> _VacationTypeService;
        public VacationController(IServicesBase<Vacation> servicesBase, IMapper mapper, IServicesBase<Employee> EmployeeService, IServicesBase<VacationType> vacationTypeService)
            : base(servicesBase, mapper)
        {
            _EmployeeService = EmployeeService;
            _VacationTypeService = vacationTypeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await PopulateVacationViewModel();
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View("Form", await PopulateVacationViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Create(VacationFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var Vacation = _mapper.Map<Vacation>(viewModel);
            await BussinesService.InsertAsync(Vacation);
            return RedirectToAction("Index", _mapper.Map<VacationViewModel>(Vacation));
        }
        [HttpGet]
        public async Task<IActionResult> EditAsync(int id)
        {
            var Vacation = await BussinesService.GetAsync(d => d.Id == id);
            if (Vacation == null)
                return NotFound();
            var viewModel = _mapper.Map<VacationFormViewModel>(Vacation);
            return View("Form", await PopulateVacationViewModel(viewModel));
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditAsync(VacationFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var Vacation = await BussinesService.GetAsync(d => d.Id == viewModel.Id);
            if (Vacation == null)
                return NotFound();
            _mapper.Map(viewModel, Vacation);

            await BussinesService.UpdateAsync(Vacation.Id, Vacation);
            return RedirectToAction("Index", _mapper.Map<VacationViewModel>(Vacation));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var Vacation = await BussinesService.GetAsync(d => d.Id == id);
            if (Vacation == null)
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
        [HttpPost]
        public async Task<IActionResult> GetVacations()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var skip = int.Parse(Request.Form["start"]!);
            var pageSize = int.Parse(Request.Form["length"]!);
            var searchValue = Request.Form["search[value]"];
            var sortColumnIndex = Request.Form["order[0][column]"];
            var sortColumn = Request.Form[$"columns[{sortColumnIndex}][name]"];
            var sortColumnDirection = Request.Form["order[0][dir]"];

            IQueryable<Vacation> VacationQuery;
            VacationQuery = (IQueryable<Vacation>)await BussinesService.GetAllAsync(null!, ["Employee", "VacationType"]);

            if (!string.IsNullOrEmpty(searchValue))
            {
                VacationQuery = VacationQuery.Where(b => b.Employee.FullName.Contains(searchValue!)
                || (b.VacationType.Name == null || b.VacationType.Name.Contains(searchValue!))
                || b.VacationDays.ToString().Contains(searchValue!)
                || (b.Description == null || b.Description.Contains(searchValue!)));
            }

            var Vacation = VacationQuery.ToList();

            // Apply sorting in-memory based on known property names
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                switch (sortColumn)
                {
                    case "Employee.FullName":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.Employee.FullName).ToList() : Vacation.OrderByDescending(b => b.Employee.FullName).ToList();
                        break;
                    case "VacationType.Name":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.VacationType.Name).ToList() : Vacation.OrderByDescending(b => b.VacationType.Name).ToList();
                        break;
                    case "Description":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.Description).ToList() : Vacation.OrderByDescending(b => b.Description).ToList();
                        break;
                    case "VacationDays":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.VacationDays).ToList() : Vacation.OrderByDescending(b => b.VacationDays).ToList();
                        break;
                    case "IsReturned":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.IsReturned).ToList() : Vacation.OrderByDescending(b => b.IsReturned).ToList();
                        break;
                    case "StartDate":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.StartDate).ToList() : Vacation.OrderByDescending(b => b.StartDate).ToList();
                        break;
                    case "ReturnedDate":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.ReturnedDate).ToList() : Vacation.OrderByDescending(b => b.ReturnedDate).ToList();
                        break;
                    default:
                        Vacation = Vacation.OrderByDescending(b => b.DateCreated).ToList(); // Default sorting
                        break;
                }
            }
            var recordsTotal = Vacation.Count;
            Vacation = Vacation.ToList();
            var data = Vacation.Skip(skip).Take(pageSize).ToList();

            var mappedData = _mapper.Map<IEnumerable<VacationViewModel>>(Vacation);

            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = mappedData };

            return Ok(jsonData);
        }
        public async Task<IActionResult> ExportToExcelAsync(string searchValue, string sortColumn, string sortColumnDirection)
        {
            IQueryable<Vacation> VacationQuery;
            VacationQuery = (IQueryable<Vacation>)await BussinesService.GetAllAsync(null!, ["Employee", "VacationType"]);

            if (!string.IsNullOrEmpty(searchValue))
            {
                VacationQuery = VacationQuery.Where(b => b.Employee.FullName.Contains(searchValue!)
                || (b.VacationType.Name == null || b.VacationType.Name.Contains(searchValue!))
                || b.VacationDays.ToString().Contains(searchValue!)
                || (b.Description == null || b.Description.Contains(searchValue!)));
            }

            var Vacation = VacationQuery.ToList();

            // Apply sorting in-memory based on known property names
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                switch (sortColumn)
                {
                    case "Employee.FullName":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.Employee.FullName).ToList() : Vacation.OrderByDescending(b => b.Employee.FullName).ToList();
                        break;
                    case "VacationType.Name":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.VacationType.Name).ToList() : Vacation.OrderByDescending(b => b.VacationType.Name).ToList();
                        break;
                    case "Description":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.Description).ToList() : Vacation.OrderByDescending(b => b.Description).ToList();
                        break;
                    case "VacationDays":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.VacationDays).ToList() : Vacation.OrderByDescending(b => b.VacationDays).ToList();
                        break;
                    case "IsReturned":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.IsReturned).ToList() : Vacation.OrderByDescending(b => b.IsReturned).ToList();
                        break;
                    case "StartDate":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.StartDate).ToList() : Vacation.OrderByDescending(b => b.StartDate).ToList();
                        break;
                    case "ReturnedDate":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.ReturnedDate).ToList() : Vacation.OrderByDescending(b => b.ReturnedDate).ToList();
                        break;
                    default:
                        Vacation = Vacation.OrderByDescending(b => b.DateCreated).ToList(); // Default sorting
                        break;
                }
            }

            // Fetch filtered and sorted data
            var data = _mapper.Map<List<VacationViewModel>>(Vacation);

            // Create Excel file
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("سجل الاجازات"); // Arabic for "Date"

                // Manually set the Arabic headers
                worksheet.Cells[1, 1].Value = "اسم الموظف";
                worksheet.Cells[1, 2].Value = "نوع الاجازه";
                worksheet.Cells[1, 3].Value = "بدايه الاجازه";
                worksheet.Cells[1, 4].Value = "ايام الاجازه";
                worksheet.Cells[1, 5].Value = "سبب الاجازه";
                worksheet.Cells[1, 6].Value = "هل تم العوده";
                worksheet.Cells[1, 7].Value = "تاريخ العوده";

                // Load data starting from row 2 (after the headers)
                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].EmployeeName;
                    worksheet.Cells[i + 2, 2].Value = data[i].VacationType;
                    worksheet.Cells[i + 2, 3].Value = data[i].StartDate;
                    worksheet.Cells[i + 2, 3].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[i + 2, 4].Value = data[i].VacationDays;
                    worksheet.Cells[i + 2, 5].Value = data[i].Description;
                    worksheet.Cells[i + 2, 6].Value = data[i].IsReturned;
                    worksheet.Cells[i + 2, 7].Value = data[i].ReturnedDate;
                    worksheet.Cells[i + 2, 7].Style.Numberformat.Format = "dd/MM/yyyy";

                }

                var stream = new MemoryStream();
                package.SaveAs(stream);

                string excelName = $"سجل الاجازات-{DateTime.Now:dd/MM/yyyy}.xlsx"; // Filename in Arabic

                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }


        private async Task<VacationFormViewModel> PopulateVacationViewModel(VacationFormViewModel? model = null)
        {
            VacationFormViewModel viewModel = model is null ? new VacationFormViewModel() : model;

            var employees = await _EmployeeService.GetAllAsync(null!);
            viewModel.Employees = _mapper.Map<IEnumerable<SelectListItem>>(employees);

            var vacationTypes = await _VacationTypeService.GetAllAsync(null!);
            viewModel.VacationTypes = _mapper.Map<IEnumerable<SelectListItem>>(vacationTypes);
            //if (model?.FloorId > 0)
            //{
            //    var Rooms = _RoomService.GetAllAsync(x => x.Status == Status.Available && x.FloorId == model.FloorId);
            //    viewModel.Rooms = _mapper.Map<IEnumerable<SelectListItem>>(Rooms);
            //}


            return viewModel;
        }
    }
}
