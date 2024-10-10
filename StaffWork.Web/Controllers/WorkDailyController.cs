using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using StaffWork.Api.Controllers;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using StaffWork.Core.Paramaters;
using StaffWork.Infrastructure.Filters;
using StaffWork.Infrastructure.Implementations;
using System.Security.Claims;

namespace StaffWork.Web.Controllers
{
    public class WorkDailyController : ApiBaseController<WorkDaily>
    {
        public readonly IServicesBase<User> UserService;
        public readonly IServicesBase<Department> DepartmentService;
        public readonly IServicesBase<WorkType> WorkTypesService;
        private readonly UserManager<User> _userManager;

        public WorkDailyController(IServicesBase<WorkDaily> servicesBase, IMapper mapper, IServicesBase<User> userService, IServicesBase<Department> departmentService, IServicesBase<WorkType> workTypesService, UserManager<User> userManager)
            : base(servicesBase, mapper)
        {
            UserService = userService;
            DepartmentService = departmentService;
            WorkTypesService = workTypesService;
            _userManager = userManager;
        }
        public async Task<IActionResult> IndexDateAsync()
        {
            var model = new DateViewModel();

            return View(model);
        }
        public async Task<IActionResult> IndexAsync(DateViewModel dateViewModel)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await UserService.GetAsync(u => u.Id == userId, new[] { "WorkDailies", "Department" });
            var workDailies = user.WorkDailies.ToList();

            var workTypes = _mapper.Map<IEnumerable<SelectListItem>>(await WorkTypesService.GetAllAsync());

            // Create a new empty list of work dailies
            var newWorkDailies = new List<WorkDaily>
                {
                    new WorkDaily(),
                };

            // Populate the custom view model
            var model = new WorkDailyFormViewModel
            {
                User = user,
                WorkDailies = newWorkDailies, // Empty list of work dailies
                WorkTypes = workTypes,        // Work type dropdown data
                Date = dateViewModel                   // Current date
            };

            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveWorkDaily(WorkDailyFormViewModel model)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var user = await UserService.GetAsync(u => u.Id == userId, new[] { "WorkDailies" });
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Process each WorkDaily in the submitted model
            if (model.WorkDailies != null && model.WorkDailies.Any())
            {
                foreach (var workDaily in model.WorkDailies)
                {
                    // Add new WorkDaily
                    user.WorkDailies.Add(new WorkDaily
                    {
                        Note = workDaily.Note,
                        WorkTypeId = workDaily.WorkTypeId,
                        UserId = user.Id,
                        Date = workDaily.Date,
                    });

                }

                // Save changes to the database
                await _userManager.UpdateAsync(user);
            }

            return RedirectToAction("IndexDate");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var WorkDaily = await BussinesService.GetAsync(d => d.Id == id);
            if (WorkDaily == null)
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
        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> EditAsync(int id)
        {
            var WorkDaily = await BussinesService.GetAsync(d => d.Id == id);
            if (WorkDaily == null)
                return NotFound();
            var model = _mapper.Map<WorkDailyEditFormViewModel>(WorkDaily);
            var viewModel = await PopulateViewModelAsync(model);

            return PartialView("_FormEdit", viewModel);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditAsync(WorkDailyEditFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var WorkDaily = await BussinesService.GetAsync(d => d.Id == viewModel.Id);
            if (WorkDaily == null)
                return NotFound();
            _mapper.Map(viewModel, WorkDaily);

            await BussinesService.UpdateAsync(WorkDaily.Id, WorkDaily);

            return RedirectToAction("IndexDate");
        }
        [HttpPost]
        public async Task<IActionResult> GetWorkDailys(DateTime? fromDate, DateTime? toDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }
            //   var skip = int.Parse(Request.Form["start"]!);
            //    var pageSize = int.Parse(Request.Form["length"]!);
            var searchValue = Request.Form["search[value]"];
            var sortColumnIndex = Request.Form["order[0][column]"];
            var sortColumn = Request.Form[$"columns[{sortColumnIndex}][name]"];
            var sortColumnDirection = Request.Form["order[0][dir]"];

            IQueryable<WorkDaily> WorkDailyQuery;
            WorkDailyQuery = (IQueryable<WorkDaily>)await BussinesService.GetAllAsync(w => w.UserId == userId, ["User", "User.Department", "WorkType"]);

            if (!string.IsNullOrEmpty(searchValue))
            {
                WorkDailyQuery = WorkDailyQuery.Where(b => b.User!.FullName.Contains(searchValue!)
                || b.User.Department!.Name.Contains(searchValue!)
                || b.Note.Contains(searchValue!)
                || b.WorkType!.Name.Contains(searchValue!));
            }

            var WorkDaily = WorkDailyQuery.ToList();

            if (fromDate.HasValue)
                WorkDaily = WorkDaily.Where(x => x.Date >= fromDate.Value).ToList();
            if (toDate.HasValue)
                WorkDaily = WorkDaily.Where(x => x.Date <= toDate.Value).ToList();
            // Apply sorting in-memory based on known property names
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                switch (sortColumn)
                {
                    case "Date":
                        WorkDaily = sortColumnDirection == "asc" ? WorkDaily.OrderBy(b => b.Date).ToList() : WorkDaily.OrderByDescending(b => b.Date).ToList();
                        break;
                    case "FullName":
                        WorkDaily = sortColumnDirection == "asc" ? WorkDaily.OrderBy(b => b.User!.FullName).ToList() : WorkDaily.OrderByDescending(b => b.User!.FullName).ToList();
                        break;
                    case "DeptName":
                        WorkDaily = sortColumnDirection == "asc" ? WorkDaily.OrderBy(b => b.User!.Department!.Name).ToList() : WorkDaily.OrderByDescending(b => b.User!.Department!.Name).ToList();
                        break;
                    case "WorkType":
                        WorkDaily = sortColumnDirection == "asc" ? WorkDaily.OrderBy(b => b.WorkType!.Name).ToList() : WorkDaily.OrderByDescending(b => b.WorkType!.Name).ToList();
                        break;
                    case "Note":
                        WorkDaily = sortColumnDirection == "asc" ? WorkDaily.OrderBy(b => b.Note).ToList() : WorkDaily.OrderByDescending(b => b.Note).ToList();
                        break;
                    default:
                        WorkDaily = WorkDaily.OrderBy(b => b.Id).ToList(); // Default sorting
                        break;
                }
            }
            var recordsTotal = WorkDaily.Count;
            WorkDaily = WorkDaily.ToList();
            //var data = WorkDaily.Skip(skip).Take(pageSize).ToList();

            var mappedData = _mapper.Map<IEnumerable<WorkDailyViewModel>>(WorkDaily);

            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = mappedData };

            return Ok(jsonData);
        }

        public async Task<IActionResult> ExportToExcelAsync(DateTime? fromDate, DateTime? toDate, string searchValue, string sortColumn, string sortColumnDirection)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            IQueryable<WorkDaily> WorkDailyQuery;
            WorkDailyQuery = (IQueryable<WorkDaily>)await BussinesService.GetAllAsync(w => w.UserId == userId, ["User", "User.Department", "WorkType"]);

            if (!string.IsNullOrEmpty(searchValue))
            {
                WorkDailyQuery = WorkDailyQuery.Where(b => b.User!.FullName.Contains(searchValue!)
                || b.User.Department!.Name.Contains(searchValue!)
                || b.Note.Contains(searchValue!)
                || b.WorkType!.Name.Contains(searchValue!));
            }
            // Apply date filters
            if (fromDate.HasValue)
            {
                WorkDailyQuery = WorkDailyQuery.Where(x => x.Date >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                WorkDailyQuery = WorkDailyQuery.Where(x => x.Date <= toDate.Value);
            }

            // Apply sorting in-memory based on known property names
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                switch (sortColumn)
                {
                    case "Date":
                        WorkDailyQuery = sortColumnDirection == "asc" ? WorkDailyQuery.OrderBy(b => b.Date) : WorkDailyQuery.OrderByDescending(b => b.Date);
                        break;
                    case "FullName":
                        WorkDailyQuery = sortColumnDirection == "asc" ? WorkDailyQuery.OrderBy(b => b.User!.FullName) : WorkDailyQuery.OrderByDescending(b => b.User!.FullName);
                        break;
                    case "DeptName":
                        WorkDailyQuery = sortColumnDirection == "asc" ? WorkDailyQuery.OrderBy(b => b.User!.Department!.Name) : WorkDailyQuery.OrderByDescending(b => b.User!.Department!.Name);
                        break;
                    case "WorkType":
                        WorkDailyQuery = sortColumnDirection == "asc" ? WorkDailyQuery.OrderBy(b => b.WorkType!.Name) : WorkDailyQuery.OrderByDescending(b => b.WorkType!.Name);
                        break;
                    case "Note":
                        WorkDailyQuery = sortColumnDirection == "asc" ? WorkDailyQuery.OrderBy(b => b.Note) : WorkDailyQuery.OrderByDescending(b => b.Note);
                        break;
                    default:
                        WorkDailyQuery = WorkDailyQuery.OrderBy(b => b.Id); // Default sorting
                        break;
                }
            }

            // Fetch filtered and sorted data
            var WorkDaily = WorkDailyQuery.ToList();
            var data = _mapper.Map<List<WorkDailyViewModel>>(WorkDaily);

            // Create Excel file
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("الاعمال اليوميه"); // Arabic for "Date"

                // Manually set the Arabic headers
                worksheet.Cells[1, 1].Value = "اسم الموظف";
                worksheet.Cells[1, 2].Value = "القسم";
                worksheet.Cells[1, 3].Value = "التاريخ";
                worksheet.Cells[1, 4].Value = "ملاحظات";
                worksheet.Cells[1, 5].Value = "العمل";

                // Load data starting from row 2 (after the headers)
                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].FullName;
                    worksheet.Cells[i + 2, 2].Value = data[i].DeptName;
                    worksheet.Cells[i + 2, 3].Value = data[i].Date;
                    worksheet.Cells[i + 2, 3].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[i + 2, 4].Value = data[i].Note;
                    worksheet.Cells[i + 2, 5].Value = data[i].WorkType;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);

                string excelName = $"بيانات اليوميه-{DateTime.Now:dd/MM/yyyy}.xlsx"; // Filename in Arabic

                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        private async Task<WorkDailyEditFormViewModel> PopulateViewModelAsync(WorkDailyEditFormViewModel? model = null)
        {
            WorkDailyEditFormViewModel viewModel = model is null ? new WorkDailyEditFormViewModel() : model;

            var workTypes = await WorkTypesService.GetAllAsync();

            viewModel.WorkTypes = _mapper.Map<IEnumerable<SelectListItem>>(workTypes);

            return viewModel;
        }
    }
}
