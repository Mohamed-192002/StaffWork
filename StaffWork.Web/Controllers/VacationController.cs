using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis;
using OfficeOpenXml;
using StaffWork.Api.Controllers;
using StaffWork.Core.Consts;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using StaffWork.Core.Paramaters;
using StaffWork.Infrastructure.Filters;
using StaffWork.Infrastructure.Implementations;
using StaffWork.Web.Service;
using System.Security.Claims;

namespace StaffWork.Web.Controllers
{
    public class VacationController : ApiBaseController<Vacation>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public readonly IServicesBase<Notification> _NotificationService;
        public readonly IServicesBase<Employee> _EmployeeService;
        public readonly IServicesBase<VacationType> _VacationTypeService;
        public VacationController(IServicesBase<Vacation> servicesBase, IMapper mapper, IServicesBase<Employee> EmployeeService, IServicesBase<VacationType> vacationTypeService, IHubContext<NotificationHub> hubContext, IServicesBase<Notification> notificationService)
            : base(servicesBase, mapper)
        {
            _EmployeeService = EmployeeService;
            _VacationTypeService = vacationTypeService;
            _hubContext = hubContext;
            _NotificationService = notificationService;
        }

        [HttpGet]
        public IActionResult ReturnedVacation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Index()
        {
            //  await _hubContext.Clients.All.SendAsync("ReceiveNotification", "تم إرسال تنبيهات الإجازات القادمة");
            //    var model = await PopulateVacationViewModel();
            return View();
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
            if (viewModel.VacationDuration == VacationDuration.Day)
            {
                Vacation.EndDate = viewModel.StartDate.AddDays(Vacation.VacationDays);
            }
            else if (viewModel.VacationDuration == VacationDuration.Month)
            {
                Vacation.EndDate = viewModel.StartDate.AddMonths(Vacation.VacationDays);
            }
            else if (viewModel.VacationDuration == VacationDuration.Year)
            {
                Vacation.EndDate = viewModel.StartDate.AddYears(Vacation.VacationDays);
            }
            ////////////////

            if (!viewModel.IsAutoNotifi)
            {
                if (viewModel.CustomNotifiDuration == VacationDuration.Day)
                {
                    Vacation.CustomNotifiDate = Vacation.EndDate.AddDays(-Vacation.CustomNotifiBeforeDays ?? 0);
                }
                else if (viewModel.CustomNotifiDuration == VacationDuration.Month)
                {
                    Vacation.CustomNotifiDate = Vacation.EndDate.AddMonths(-Vacation.CustomNotifiBeforeDays ?? 0);
                }
                else if (viewModel.CustomNotifiDuration == VacationDuration.Year)
                {
                    Vacation.CustomNotifiDate = Vacation.EndDate.AddYears(-Vacation.CustomNotifiBeforeDays ?? 0);
                }

            }

            await BussinesService.InsertAsync(Vacation);

            #region Send Notification
            var vacation = await BussinesService.GetAsync(x => x.Id == Vacation.Id, ["Employee"]);
            if (viewModel.IsAutoNotifi)
            {
                var scheduleTime = vacation.EndDate.AddDays(-1) - DateTime.Now;
                if (scheduleTime.TotalSeconds > 0) // Ensure scheduling in the future
                {
                    BackgroundJob.Schedule<NotifiJob>(
                        x => x.ScheduleNotifiJob(vacation),
                        scheduleTime
                    );
                }
                else
                {
                    // If the EndDate is in less than a day, schedule immediately
                    BackgroundJob.Enqueue<NotifiJob>(x => x.ScheduleNotifiJob(vacation));
                }
            }
            else
            {
                // var scheduleTime = (vacation.EndDate.AddDays(-1) - vacation.CustomNotifiDate);
                var scheduleTime = vacation.CustomNotifiDate - DateTime.UtcNow;
                if (scheduleTime.TotalSeconds > 0) // Ensure scheduling in the future
                {
                    BackgroundJob.Schedule<NotifiJob>(
                        x => x.ScheduleNotifiJob(vacation),
                        scheduleTime
                    );
                }
                else
                {
                    // If the CustomNotifiDate is after EndDate, schedule immediately
                    BackgroundJob.Enqueue<NotifiJob>(x => x.ScheduleNotifiJob(vacation));
                }
            }

            #endregion

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
            if (viewModel.VacationDuration == VacationDuration.Day)
            {
                Vacation.EndDate = viewModel.StartDate.AddDays(Vacation.VacationDays);
            }
            else if (viewModel.VacationDuration == VacationDuration.Month)
            {
                Vacation.EndDate = viewModel.StartDate.AddMonths(Vacation.VacationDays);
            }
            else if (viewModel.VacationDuration == VacationDuration.Year)
            {
                Vacation.EndDate = viewModel.StartDate.AddYears(Vacation.VacationDays);
            }

            ////////////////

            if (!viewModel.IsAutoNotifi)
            {
                if (viewModel.CustomNotifiDuration == VacationDuration.Day)
                {
                    Vacation.CustomNotifiDate = Vacation.EndDate.AddDays(-Vacation.CustomNotifiBeforeDays ?? 0);
                }
                else if (viewModel.CustomNotifiDuration == VacationDuration.Month)
                {
                    Vacation.CustomNotifiDate = Vacation.EndDate.AddMonths(-Vacation.CustomNotifiBeforeDays ?? 0);
                }
                else if (viewModel.CustomNotifiDuration == VacationDuration.Year)
                {
                    Vacation.CustomNotifiDate = Vacation.EndDate.AddYears(-Vacation.CustomNotifiBeforeDays ?? 0);
                }

            }

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
                var notifications = await _NotificationService.GetAllAsync(x => x.VacationId == id);
                foreach (var notification in notifications.ToList())
                {
                    await _NotificationService.DeleteAsync(notification.Id);
                }
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
            VacationQuery = (IQueryable<Vacation>)await BussinesService.GetAllAsync(x => !x.IsReturned, ["Employee", "VacationType"], orderBy: x => x.EndDate);

            if (!string.IsNullOrEmpty(searchValue))
            {
                VacationQuery = VacationQuery.Where(
                    b => b.Employee.FullName.Contains(searchValue!)
                    || (b.Employee.Court != null && b.Employee.Court.Contains(searchValue!))
                    || (b.Employee.Appeal != null && b.Employee.Appeal.Contains(searchValue!))
                    || (b.VacationType.Name != null && b.VacationType.Name.Contains(searchValue!))
                    //  || b.VacationDuration.ToString().Contains(searchValue!)
                    || b.VacationDays.ToString().Contains(searchValue!)
                    || (b.Description != null && b.Description.Contains(searchValue!))
                );
            }


            var Vacation = VacationQuery.ToList();

            // Apply sorting in-memory based on known property names
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                switch (sortColumn)
                {
                    case "EmployeeName":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.Employee.FullName).ToList() : Vacation.OrderByDescending(b => b.Employee.FullName).ToList();
                        break;
                    case "Court":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.Employee.Court).ToList() : Vacation.OrderByDescending(b => b.Employee.Court).ToList();
                        break;
                    case "Appeal":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.Employee.Appeal).ToList() : Vacation.OrderByDescending(b => b.Employee.Appeal).ToList();
                        break;
                    case "VacationType":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.VacationType.Name).ToList() : Vacation.OrderByDescending(b => b.VacationType.Name).ToList();
                        break;
                    case "Description":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.Description).ToList() : Vacation.OrderByDescending(b => b.Description).ToList();
                        break;
                    case "VacationDays":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.VacationDays).ToList() : Vacation.OrderByDescending(b => b.VacationDays).ToList();
                        break;
                    case "VacationDuration":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.VacationDuration).ToList() : Vacation.OrderByDescending(b => b.VacationDuration).ToList();
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
                        Vacation = Vacation.OrderBy(b => b.EndDate).ToList(); // Default sorting
                        break;
                }
            }
            var recordsTotal = Vacation.Count;
            Vacation = Vacation.ToList();
            var data = Vacation.Skip(skip).Take(pageSize).ToList();

            var mappedData = _mapper.Map<IEnumerable<VacationViewModel>>(data);

            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = mappedData };

            return Ok(jsonData);
        }
        public async Task<IActionResult> ExportToExcelAsync(string searchValue, string sortColumn, string sortColumnDirection)
        {
            IQueryable<Vacation> VacationQuery;
            VacationQuery = (IQueryable<Vacation>)await BussinesService.GetAllAsync(null!, ["Employee", "VacationType"]);

            if (!string.IsNullOrEmpty(searchValue))
            {
                VacationQuery = VacationQuery.Where(
                   b => b.Employee.FullName.Contains(searchValue!)
                   || (b.Employee.Court != null && b.Employee.Court.Contains(searchValue!))
                   || (b.Employee.Appeal != null && b.Employee.Appeal.Contains(searchValue!))
                   || (b.VacationType.Name != null && b.VacationType.Name.Contains(searchValue!))
                   //  || b.VacationDuration.ToString().Contains(searchValue!)
                   || b.VacationDays.ToString().Contains(searchValue!)
                   || (b.Description != null && b.Description.Contains(searchValue!))
               );
            }

            var Vacation = VacationQuery.ToList();

            // Apply sorting in-memory based on known property names
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                switch (sortColumn)
                {
                    case "EmployeeName":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.Employee.FullName).ToList() : Vacation.OrderByDescending(b => b.Employee.FullName).ToList();
                        break;
                    case "Court":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.Employee.Court).ToList() : Vacation.OrderByDescending(b => b.Employee.Court).ToList();
                        break;
                    case "Appeal":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.Employee.Appeal).ToList() : Vacation.OrderByDescending(b => b.Employee.Appeal).ToList();
                        break;
                    case "VacationType":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.VacationType.Name).ToList() : Vacation.OrderByDescending(b => b.VacationType.Name).ToList();
                        break;
                    case "Description":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.Description).ToList() : Vacation.OrderByDescending(b => b.Description).ToList();
                        break;
                    case "VacationDays":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.VacationDays).ToList() : Vacation.OrderByDescending(b => b.VacationDays).ToList();
                        break;
                    case "VacationDuration":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.VacationDuration).ToList() : Vacation.OrderByDescending(b => b.VacationDuration).ToList();
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
                worksheet.Cells[1, 2].Value = "دار القضاء";
                worksheet.Cells[1, 3].Value = "الاستئناف";
                worksheet.Cells[1, 4].Value = "نوع الاجازه";
                worksheet.Cells[1, 5].Value = "بدايه الاجازه";
                worksheet.Cells[1, 6].Value = "مده الاجازه";
                worksheet.Cells[1, 7].Value = "فتره الاجازه";
                worksheet.Cells[1, 8].Value = "سبب الاجازه";
                worksheet.Cells[1, 9].Value = "هل تم المباشرة";
                worksheet.Cells[1, 10].Value = "تاريخ المباشرة";
                worksheet.Cells[1, 11].Value = "تاريخ المباشرة المتوقع";

                // Load data starting from row 2 (after the headers)
                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].EmployeeName;
                    worksheet.Cells[i + 2, 2].Value = data[i].Court;
                    worksheet.Cells[i + 2, 3].Value = data[i].Appeal;
                    worksheet.Cells[i + 2, 4].Value = data[i].VacationType;
                    worksheet.Cells[i + 2, 5].Value = data[i].StartDate;
                    worksheet.Cells[i + 2, 5].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[i + 2, 6].Value = data[i].VacationDays;
                    worksheet.Cells[i + 2, 7].Value = data[i].VacationDuration == VacationDuration.Day ? "يوم" : data[i].VacationDuration == VacationDuration.Month ? "شهر" : "سنه";
                    worksheet.Cells[i + 2, 8].Value = data[i].Description;
                    worksheet.Cells[i + 2, 9].Value = data[i].IsReturned;
                    worksheet.Cells[i + 2, 10].Value = data[i].ReturnedDate;
                    worksheet.Cells[i + 2, 10].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[i + 2, 11].Value = data[i].EndDate;
                    worksheet.Cells[i + 2, 11].Style.Numberformat.Format = "dd/MM/yyyy";

                }

                var stream = new MemoryStream();
                package.SaveAs(stream);

                string excelName = $"سجل الاجازات-{DateTime.Now:dd/MM/yyyy}.xlsx"; // Filename in Arabic

                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetReturnedVacations()
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
            VacationQuery = (IQueryable<Vacation>)await BussinesService.GetAllAsync(x => x.IsReturned, ["Employee", "VacationType"]);

            if (!string.IsNullOrEmpty(searchValue))
            {
                VacationQuery = VacationQuery.Where(
                    b => b.Employee.FullName.Contains(searchValue!)
                    || (b.Employee.Court != null && b.Employee.Court.Contains(searchValue!))
                    || (b.Employee.Appeal != null && b.Employee.Appeal.Contains(searchValue!))
                    || (b.VacationType.Name != null && b.VacationType.Name.Contains(searchValue!))
                    //  || b.VacationDuration.ToString().Contains(searchValue!)
                    || b.VacationDays.ToString().Contains(searchValue!)
                    || (b.Description != null && b.Description.Contains(searchValue!))
                );
            }


            var Vacation = VacationQuery.ToList();

            // Apply sorting in-memory based on known property names
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                switch (sortColumn)
                {
                    case "EmployeeName":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.Employee.FullName).ToList() : Vacation.OrderByDescending(b => b.Employee.FullName).ToList();
                        break;
                    case "Court":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.Employee.Court).ToList() : Vacation.OrderByDescending(b => b.Employee.Court).ToList();
                        break;
                    case "Appeal":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.Employee.Appeal).ToList() : Vacation.OrderByDescending(b => b.Employee.Appeal).ToList();
                        break;
                    case "VacationType":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.VacationType.Name).ToList() : Vacation.OrderByDescending(b => b.VacationType.Name).ToList();
                        break;
                    case "Description":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.Description).ToList() : Vacation.OrderByDescending(b => b.Description).ToList();
                        break;
                    case "VacationDays":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.VacationDays).ToList() : Vacation.OrderByDescending(b => b.VacationDays).ToList();
                        break;
                    case "VacationDuration":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.VacationDuration).ToList() : Vacation.OrderByDescending(b => b.VacationDuration).ToList();
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

            var mappedData = _mapper.Map<IEnumerable<VacationViewModel>>(data);

            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = mappedData };

            return Ok(jsonData);
        }
        public async Task<IActionResult> ReturnedVacationExportToExcelAsync(string searchValue, string sortColumn, string sortColumnDirection)
        {
            IQueryable<Vacation> VacationQuery;
            VacationQuery = (IQueryable<Vacation>)await BussinesService.GetAllAsync(x => x.IsReturned, ["Employee", "VacationType"]);

            if (!string.IsNullOrEmpty(searchValue))
            {
                VacationQuery = VacationQuery.Where(
                   b => b.Employee.FullName.Contains(searchValue!)
                   || (b.Employee.Court != null && b.Employee.Court.Contains(searchValue!))
                   || (b.Employee.Appeal != null && b.Employee.Appeal.Contains(searchValue!))
                   || (b.VacationType.Name != null && b.VacationType.Name.Contains(searchValue!))
                   //  || b.VacationDuration.ToString().Contains(searchValue!)
                   || b.VacationDays.ToString().Contains(searchValue!)
                   || (b.Description != null && b.Description.Contains(searchValue!))
               );
            }

            var Vacation = VacationQuery.ToList();

            // Apply sorting in-memory based on known property names
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                switch (sortColumn)
                {
                    case "EmployeeName":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.Employee.FullName).ToList() : Vacation.OrderByDescending(b => b.Employee.FullName).ToList();
                        break;
                    case "Court":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.Employee.Court).ToList() : Vacation.OrderByDescending(b => b.Employee.Court).ToList();
                        break;
                    case "Appeal":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.Employee.Appeal).ToList() : Vacation.OrderByDescending(b => b.Employee.Appeal).ToList();
                        break;
                    case "VacationType":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.VacationType.Name).ToList() : Vacation.OrderByDescending(b => b.VacationType.Name).ToList();
                        break;
                    case "Description":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.Description).ToList() : Vacation.OrderByDescending(b => b.Description).ToList();
                        break;
                    case "VacationDays":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.VacationDays).ToList() : Vacation.OrderByDescending(b => b.VacationDays).ToList();
                        break;
                    case "VacationDuration":
                        Vacation = sortColumnDirection == "asc" ? Vacation.OrderBy(b => b.VacationDuration).ToList() : Vacation.OrderByDescending(b => b.VacationDuration).ToList();
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
                worksheet.Cells[1, 2].Value = "دار القضاء";
                worksheet.Cells[1, 3].Value = "الاستئناف";
                worksheet.Cells[1, 4].Value = "نوع الاجازه";
                worksheet.Cells[1, 5].Value = "بدايه الاجازه";
                worksheet.Cells[1, 6].Value = "مده الاجازه";
                worksheet.Cells[1, 7].Value = "فتره الاجازه";
                worksheet.Cells[1, 8].Value = "سبب الاجازه";
                worksheet.Cells[1, 9].Value = "هل تم المباشرة";
                worksheet.Cells[1, 10].Value = "تاريخ المباشرة";
                worksheet.Cells[1, 11].Value = "تاريخ المباشرة المتوقع";

                // Load data starting from row 2 (after the headers)
                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].EmployeeName;
                    worksheet.Cells[i + 2, 2].Value = data[i].Court;
                    worksheet.Cells[i + 2, 3].Value = data[i].Appeal;
                    worksheet.Cells[i + 2, 4].Value = data[i].VacationType;
                    worksheet.Cells[i + 2, 5].Value = data[i].StartDate;
                    worksheet.Cells[i + 2, 5].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[i + 2, 6].Value = data[i].VacationDays;
                    worksheet.Cells[i + 2, 7].Value = data[i].VacationDuration == VacationDuration.Day ? "يوم" : data[i].VacationDuration == VacationDuration.Month ? "شهر" : "سنه";
                    worksheet.Cells[i + 2, 8].Value = data[i].Description;
                    worksheet.Cells[i + 2, 9].Value = data[i].IsReturned;
                    worksheet.Cells[i + 2, 10].Value = data[i].ReturnedDate;
                    worksheet.Cells[i + 2, 10].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[i + 2, 11].Value = data[i].EndDate;
                    worksheet.Cells[i + 2, 11].Style.Numberformat.Format = "dd/MM/yyyy";

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

            //var employees = await _EmployeeService.GetAllAsync(null!);
            //viewModel.Employees = _mapper.Map<IEnumerable<SelectListItem>>(employees);

            var vacationTypes = await _VacationTypeService.GetAllAsync(null!);
            viewModel.VacationTypes = _mapper.Map<IEnumerable<SelectListItem>>(vacationTypes);
            //if (model?.FloorId > 0)
            //{
            //    var Rooms = _RoomService.GetAllAsync(x => x.Status == Status.Available && x.FloorId == model.FloorId);
            //    viewModel.Rooms = _mapper.Map<IEnumerable<SelectListItem>>(Rooms);
            //}


            return viewModel;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetEmployees(string search, int page = 1, int pageSize = 10, int? selectedId = null)
        {
            var query = await _EmployeeService.GetAllAsync(null!);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.FullName.Contains(search));
            }

            // If an employee is selected (for edit mode), fetch it explicitly
            if (selectedId.HasValue)
            {
                var selectedEmployee = query.FirstOrDefault(e => e.Id == selectedId.Value);
                if (selectedEmployee != null)
                {
                    return Json(new
                    {
                        id = selectedEmployee.Id,
                        name = selectedEmployee.FullName
                    });
                }
            }

            var totalCount = query.Count();
            var employees = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return Json(new
            {
                items = employees.Select(e => new { id = e.Id, name = e.FullName }),
                hasMore = (page * pageSize) < totalCount
            });
        }



    }
}
