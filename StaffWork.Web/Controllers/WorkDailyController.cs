using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using StaffWork.Api.Controllers;
using StaffWork.Core.Consts;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using StaffWork.Core.Paramaters;
using StaffWork.Infrastructure.Filters;
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

            var selectedDate = dateViewModel.Date.Date; // فقط التاريخ بدون وقت
            var currentTime = DateTime.Now.TimeOfDay; // الوقت فقط
            dateViewModel.Date = selectedDate + currentTime;
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
                    var work = new WorkDaily
                    {
                        Note = workDaily.Note,
                        WorkTypeId = workDaily.WorkTypeId,
                        UserId = user.Id,
                        Date = workDaily.Date,
                        DateCreated = DateTime.Now,
                    };
                    await BussinesService.InsertAsync(work);

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
                return Unauthorized();
            var user = await UserService.GetAsync(x => x.Id == userId, ["Department"]);

            //   var skip = int.Parse(Request.Form["start"]!);
            //    var pageSize = int.Parse(Request.Form["length"]!);
            var searchValue = Request.Form["search[value]"];
            var sortColumnIndex = Request.Form["order[0][column]"];
            var sortColumn = Request.Form[$"columns[{sortColumnIndex}][name]"];
            var sortColumnDirection = Request.Form["order[0][dir]"];

            IQueryable<WorkDaily> WorkDailyQuery;

            if (User.IsInRole(AppRoles.SuperAdmin))
                WorkDailyQuery = (IQueryable<WorkDaily>)await BussinesService.GetAllAsync(null!, ["User", "User.Department", "WorkType"]);
            else if (User.IsInRole(AppRoles.Admin))
                WorkDailyQuery = (IQueryable<WorkDaily>)await BussinesService.GetAllAsync(w => w.User!.DepartmentId == user.DepartmentId, ["User", "User.Department", "WorkType"]);

            else
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
            foreach (var item in mappedData)
            {
                var createdDate = item.DateCreated;
                var currentDate = DateTime.UtcNow;
                var timeDifference = currentDate - createdDate;
                var daysDifference = timeDifference.TotalDays;
                if (daysDifference < 0) daysDifference = -daysDifference;
                var isAdmin = !User.IsInRole(AppRoles.SuperAdmin);
                //  item.IsDisabled = ((item.Status == Status.Pending.ToString()) && daysDifference > 7) && isAdmin;
            }
            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = mappedData };

            return Ok(jsonData);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptStatusAsync(int id)
        {
            var work = await BussinesService.GetAsync(d => d.Id == id);

            if (work is null)
                return NotFound();

            if (!User.IsInRole(AppRoles.SuperAdmin) || work.Status != Status.Accepted)
            {
                // Check if the work item has been pending for more than 7 days 
                var daysPending = (DateTime.UtcNow - work.DateCreated).TotalDays; // Assuming PendingDate is the date the item was set to pending
                if (daysPending > 7)
                {
                    return BadRequest(new { Message = "This item cannot be accepted after 7 days of pending status." });
                }
            }

            work.Status = Status.Accepted;

            await BussinesService.UpdateAsync(work.Id, work);

            return Ok();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            var work = await BussinesService.GetAsync(d => d.Id == id);

            if (work is null)
                return NotFound();

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (work.IsCompleted)
            {
                return BadRequest(new { Message = "This item is already completed." });
            }

            if (!(User.IsInRole(AppRoles.SuperAdmin) || currentUserId == work.UserId))
            {
                return Forbid("you're not authorized");
            }

            work.IsCompleted = true;
            work.CompletionDate = DateTime.Now;
            TimeDifferenceFormatted(work);
            await BussinesService.UpdateAsync(work.Id, work);

            return Ok();
        }

        private static void TimeDifferenceFormatted(WorkDaily work)
        {
            var start = work.Date;
            var end = work.CompletionDate.Value;

            var duration = end - start;

            // عدد السنين والشهور
            int years = end.Year - start.Year;
            int months = end.Month - start.Month;
            if (months < 0)
            {
                years--;
                months += 12;
            }

            // عدد الأيام والساعات والدقائق
            var baseDate = start.AddYears(years).AddMonths(months);
            int days = (end - baseDate).Days;

            var timePart = end - baseDate.AddDays(days);
            int hours = timePart.Hours;
            int minutes = timePart.Minutes;

            // تركيب النص العربي
            var parts = new List<string>();
            if (years > 0) parts.Add($"{years} {(years == 1 ? "سنة" : years == 2 ? "سنتين" : years <= 10 ? "سنوات" : "سنة")}");
            if (months > 0) parts.Add($"{months} {(months == 1 ? "شهر" : months == 2 ? "شهرين" : months <= 10 ? "شهور" : "شهراً")}");
            if (days > 0) parts.Add($"{days} {(days == 1 ? "يوم" : days == 2 ? "يومين" : days <= 10 ? "أيام" : "يوماً")}");
            if (hours > 0) parts.Add($"{hours} {(hours == 1 ? "ساعة" : hours == 2 ? "ساعتين" : hours <= 10 ? "ساعات" : "ساعة")}");
            if (minutes > 0) parts.Add($"{minutes} {(minutes == 1 ? "دقيقة" : minutes == 2 ? "دقيقتين" : minutes <= 10 ? "دقائق" : "دقيقة")}");

            // دمج النص
            work.TimeDifferenceFormatted = parts.Count > 0 ? string.Join(" و", parts) : "أقل من دقيقة";
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectedStatusAsync(int id)
        {
            var work = await BussinesService.GetAsync(d => d.Id == id);

            if (work is null)
                return NotFound();

            work.Status = Status.Rejected;

            await BussinesService.UpdateAsync(work.Id, work);

            return Ok();
        }
        public async Task<IActionResult> ExportToExcelAsync(DateTime? fromDate, DateTime? toDate, string searchValue, string sortColumn, string sortColumnDirection)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var user = await UserService.GetAsync(x => x.Id == userId, ["Department"]);
            IQueryable<WorkDaily> WorkDailyQuery;

            if (User.IsInRole(AppRoles.SuperAdmin))
                WorkDailyQuery = (IQueryable<WorkDaily>)await BussinesService.GetAllAsync(null!, ["User", "User.Department", "WorkType"]);
            else if (User.IsInRole(AppRoles.Admin))
                WorkDailyQuery = (IQueryable<WorkDaily>)await BussinesService.GetAllAsync(w => w.User!.DepartmentId == user.DepartmentId, ["User", "User.Department", "WorkType"]);

            else
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
                worksheet.Cells[1, 6].Value = "هل انجزت";
                worksheet.Cells[1, 7].Value = "تاريخ الانجاز";
                worksheet.Cells[1, 8].Value = "مده العمل";

                // Load data starting from row 2 (after the headers)
                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].FullName;
                    worksheet.Cells[i + 2, 2].Value = data[i].DeptName;
                    worksheet.Cells[i + 2, 3].Value = data[i].Date;
                    worksheet.Cells[i + 2, 3].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[i + 2, 4].Value = data[i].Note;
                    worksheet.Cells[i + 2, 5].Value = data[i].WorkType;
                    worksheet.Cells[i + 2, 6].Value = data[i].IsCompleted ? "نعم" : "لا";
                    worksheet.Cells[i + 2, 7].Value = data[i].CompletionDate;
                    worksheet.Cells[i + 2, 7].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[i + 2, 8].Value = data[i].TimeDifferenceFormatted;

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
