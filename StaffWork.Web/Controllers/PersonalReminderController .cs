using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using StaffWork.Api.Controllers;
using StaffWork.Core.Consts;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using StaffWork.Core.Paramaters;
using Hangfire;
using OfficeOpenXml;

namespace StaffWork.Web.Controllers
{
    public class PersonalReminderController : ApiBaseController<PersonalReminder>
    {
        public readonly IServicesBase<User> UserService;
        public readonly IServicesBase<TaskFile> TaskFileService;
        public readonly IServicesBase<PersonalReminderFile> PersonalReminderFileService;
        public readonly IServicesBase<Notification> NotificationService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public PersonalReminderController(IServicesBase<PersonalReminder> servicesBase, IMapper mapper, IServicesBase<User> userService, IServicesBase<TaskFile> taskFileService, IServicesBase<PersonalReminderFile> personalReminderFileService, IServicesBase<Notification> notificationService, IWebHostEnvironment webHostEnvironment) : base(servicesBase, mapper)
        {
            UserService = userService;
            TaskFileService = taskFileService;
            PersonalReminderFileService = personalReminderFileService;
            NotificationService = notificationService;
            _webHostEnvironment = webHostEnvironment;
        }

        private string GetAuthenticatedUser()
        {
            var userUidClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userUidClaim?.Value!;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create(int taskModelId)
        {
            var viewModel = new PersonalReminderFormViewModel
            {
                ReminderDate = DateTime.UtcNow
            };
            return View("Form", viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Create(PersonalReminderFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var PersonalReminder = _mapper.Map<PersonalReminder>(viewModel);
            PersonalReminder.CreatedByUserId = GetAuthenticatedUser();
            // Handle Book images
            if (viewModel.PersonalReminderFormFiles != null && viewModel.PersonalReminderFormFiles.Count > 0)
            {
                foreach (var image in viewModel.PersonalReminderFormFiles)
                {
                    if (image.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                        var filePath = "/files/PersonalReminders";
                        if (!Directory.Exists($"{_webHostEnvironment.WebRootPath}{filePath}"))
                            Directory.CreateDirectory($"{_webHostEnvironment.WebRootPath}{filePath}");

                        var path = Path.Combine($"{_webHostEnvironment.WebRootPath}{filePath}", fileName);

                        using var stream = System.IO.File.Create(path);
                        image.CopyTo(stream);

                        var PersonalReminderFile = new PersonalReminderFile
                        {
                            FileUrl = $"{filePath}/{fileName}",
                            FileName = image.FileName,
                            PersonalReminder = PersonalReminder
                        };
                        PersonalReminder.PersonalReminderFiles.Add(PersonalReminderFile);
                    }
                }
            }
            await BussinesService.InsertAsync(PersonalReminder);
            #region Send Notification
            var reminder = await BussinesService.GetAsync(x => x.Id == PersonalReminder.Id);
            var reminderDto = _mapper.Map<PersonalReminderViewModel>(reminder);
            var scheduleTime = reminder.ReminderDate.AddDays(-1) - DateTime.Now;
            if (scheduleTime.TotalSeconds > 0)
            {
                reminder.JobId = BackgroundJob.Schedule<NotifiJob>(
                    x => x.ScheduleNotifiPersonalJob(reminderDto),
                    scheduleTime
                );
            }
            else
            {
                reminder.JobId = BackgroundJob.Enqueue<NotifiJob>(x => x.ScheduleNotifiPersonalJob(reminderDto));
            }
            #endregion
            return RedirectToAction("Index", _mapper.Map<PersonalReminderViewModel>(PersonalReminder));
        }
        [HttpGet]
        public async Task<IActionResult> EditAsync(int id)
        {
            var PersonalReminder = await BussinesService.GetAsync(d => d.Id == id, ["PersonalReminderFiles"]);
            if (PersonalReminder == null)
                return NotFound();
            var viewModel = _mapper.Map<PersonalReminderFormViewModel>(PersonalReminder);
            viewModel.ExistingFiles = _mapper.Map<List<TaskFileDisplay>>(PersonalReminder.PersonalReminderFiles);
            return View("Form", PopulateViewModel(viewModel));
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditAsync(PersonalReminderFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var PersonalReminder = await BussinesService.GetAsync(d => d.Id == viewModel.Id);
            if (PersonalReminder == null)
                return NotFound();
            _mapper.Map(viewModel, PersonalReminder);
            #region Files List
            // Handle file uploads
            if (viewModel.PersonalReminderFormFiles.Any())
            {
                foreach (var file in viewModel.PersonalReminderFormFiles)
                {
                    if (file.Length > 0)
                    {

                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var filePath = "/files/PersonalReminders";
                        if (!Directory.Exists($"{_webHostEnvironment.WebRootPath}{filePath}"))
                            Directory.CreateDirectory($"{_webHostEnvironment.WebRootPath}{filePath}");

                        var path = Path.Combine($"{_webHostEnvironment.WebRootPath}{filePath}", fileName);

                        using var stream = System.IO.File.Create(path);
                        await file.CopyToAsync(stream);

                        var PersonalReminderFile = new PersonalReminderFile
                        {
                            FileUrl = $"{filePath}/{fileName}",
                            FileName = file.FileName,
                            PersonalReminder = PersonalReminder
                        };
                        PersonalReminder.PersonalReminderFiles.Add(PersonalReminderFile);
                    }
                }
            }

            // Handle image deletions
            var deletedImageUrls = viewModel.DeletedFileUrls?.Split(',') ?? Array.Empty<string>();
            foreach (var imageUrl in deletedImageUrls)
            {
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    var file = await TaskFileService.GetAsync(r => r.FileUrl == imageUrl);
                    if (file != null)
                    {
                        await TaskFileService.DeleteAsync(file.Id);
                    }

                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
            }
            #endregion
            await BussinesService.UpdateAsync(PersonalReminder.Id, PersonalReminder);

            #region Send Notification
            var reminder = await BussinesService.GetAsync(x => x.Id == PersonalReminder.Id);
            var reminderDto = _mapper.Map<PersonalReminderViewModel>(reminder);
            if (reminder.JobId != null)
            {
                BackgroundJob.Delete(reminder.JobId);
            }
            if (!reminder.IsReminderCompleted)
            {
                var scheduleTime = reminder.ReminderDate.AddDays(-1) - DateTime.Now;
                if (scheduleTime.TotalSeconds > 0)
                {
                    reminder.JobId = BackgroundJob.Schedule<NotifiJob>(
                        x => x.ScheduleNotifiPersonalJob(reminderDto),
                        scheduleTime
                    );
                }
                else
                {
                    reminder.JobId = BackgroundJob.Enqueue<NotifiJob>(x => x.ScheduleNotifiPersonalJob(reminderDto));
                }
            }
            #endregion
            return RedirectToAction("Index", _mapper.Map<PersonalReminderViewModel>(PersonalReminder));
        }
        [Authorize(Roles = AppRoles.Admin + "," + AppRoles.SuperAdmin)]
        public async Task<IActionResult> Delete(int id)
        {
            var PersonalReminder = await BussinesService.GetAsync(d => d.Id == id, ["Notifications", "PersonalReminderFiles"]);
            if (PersonalReminder == null)
                return NotFound();
            try
            {
                if (PersonalReminder.JobId != null)
                {
                    BackgroundJob.Delete(PersonalReminder.JobId);
                }
                foreach (var item in PersonalReminder.Notifications.ToList())
                {
                    await NotificationService.DeleteAsync(item.Id);
                }
                foreach (var item in PersonalReminder.PersonalReminderFiles.ToList())
                {
                    await PersonalReminderFileService.DeleteAsync(item.Id);
                }
                await BussinesService.DeleteAsync(id);
            }
            catch (Exception)
            {
                throw;
            }
            return Ok();
        }
        public async Task<IActionResult> Complete(int Id)
        {
            var currentUserId = GetAuthenticatedUser();
            var PersonalReminder = await BussinesService.GetAsync(d => d.Id == Id);
            if (PersonalReminder == null)
                return NotFound();
            try
            {
                if (PersonalReminder.IsReminderCompleted)
                {
                    return BadRequest(new { Message = "This item is already completed." });
                }

                if (PersonalReminder.CreatedByUserId != currentUserId)
                {
                    return Forbid("you're not authorized");
                }
                PersonalReminder.IsReminderCompleted = true;
                PersonalReminder.ReminderCompletedDate = DateTime.UtcNow;
                if (PersonalReminder.JobId != null)
                {
                    BackgroundJob.Delete(PersonalReminder.JobId);
                }
                await BussinesService.UpdateAsync(PersonalReminder.Id, PersonalReminder);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private PersonalReminderFormViewModel PopulateViewModel(PersonalReminderFormViewModel? model = null)
        {
            var userId = GetAuthenticatedUser();
            var user = UserService.GetAsync(x => x.Id == userId, ["Department"]).Result;

            PersonalReminderFormViewModel viewModel = model is null ? new PersonalReminderFormViewModel() : model;

            return viewModel;
        }

        [HttpPost]
        public async Task<IActionResult> GetPersonalReminders(DateTime? fromDate, DateTime? toDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var user = await UserService.GetAsync(x => x.Id == userId, ["Department"]);

            var skip = int.Parse(Request.Form["start"]!);
            var pageSize = int.Parse(Request.Form["length"]!);
            var searchValue = Request.Form["search[value]"];
            var sortColumnIndex = Request.Form["order[0][column]"];
            var sortColumn = Request.Form[$"columns[{sortColumnIndex}][name]"];
            var sortColumnDirection = Request.Form["order[0][dir]"];

            IQueryable<PersonalReminder> PersonalReminderQuery;
            PersonalReminderQuery = (IQueryable<PersonalReminder>)await BussinesService
                                 .GetAllAsync(w => !w.IsReminderCompleted && w.CreatedByUserId == user.Id
                                 , ["CreatedByUser"]);


            if (!string.IsNullOrEmpty(searchValue))
            {
                PersonalReminderQuery = PersonalReminderQuery.Where(b =>
                 (string.IsNullOrEmpty(b.Title) || b.Title.Contains(searchValue!))
                || (string.IsNullOrEmpty(b.Notes) || b.Notes.Contains(searchValue!))
                || (string.IsNullOrEmpty(b.CreatedByUser.FullName) || b.CreatedByUser.FullName.Contains(searchValue!))
                );
            }

            var PersonalReminder = PersonalReminderQuery.ToList();

            if (fromDate.HasValue)
                PersonalReminder = PersonalReminder.Where(x => x.DateCreated >= fromDate.Value).ToList();
            if (toDate.HasValue)
                PersonalReminder = PersonalReminder.Where(x => x.DateCreated <= toDate.Value).ToList();
            // Apply sorting in-memory based on known property names
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                switch (sortColumn)
                {
                    case "Title":
                        PersonalReminder = sortColumnDirection == "asc" ? PersonalReminder.OrderBy(b => b.Title).ToList() : PersonalReminder.OrderByDescending(b => b.Title).ToList();
                        break;
                    case "Notes":
                        PersonalReminder = sortColumnDirection == "asc" ? PersonalReminder.OrderBy(b => b.Notes).ToList() : PersonalReminder.OrderByDescending(b => b.Notes).ToList();
                        break;
                    case "CreatedByUserName":
                        PersonalReminder = sortColumnDirection == "asc" ? PersonalReminder.OrderBy(b => b.CreatedByUser.FullName).ToList() : PersonalReminder.OrderByDescending(b => b.CreatedByUser.FullName).ToList();
                        break;
                    case "ReminderDate":
                        PersonalReminder = sortColumnDirection == "asc" ? PersonalReminder.OrderBy(b => b.ReminderDate).ToList() : PersonalReminder.OrderByDescending(b => b.ReminderDate).ToList();
                        break;
                    case "IsReminderCompleted":
                        PersonalReminder = sortColumnDirection == "asc" ? PersonalReminder.OrderBy(b => b.IsReminderCompleted).ToList() : PersonalReminder.OrderByDescending(b => b.IsReminderCompleted).ToList();
                        break;
                    case "ReminderCompletedDate":
                        PersonalReminder = sortColumnDirection == "asc" ? PersonalReminder.OrderBy(b => b.ReminderCompletedDate).ToList() : PersonalReminder.OrderByDescending(b => b.ReminderCompletedDate).ToList();
                        break;
                    default:
                        PersonalReminder = PersonalReminder.OrderBy(b => b.Id).ToList(); // Default sorting
                        break;
                }
            }
            var recordsTotal = PersonalReminder.Count;
            PersonalReminder = PersonalReminder.ToList();
            var data = PersonalReminder.Skip(skip).Take(pageSize).ToList();

            var mappedData = _mapper.Map<IEnumerable<PersonalReminderViewModel>>(PersonalReminder);


            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = mappedData };

            return Ok(jsonData);
        }

        public async Task<IActionResult> ExportToExcelAsync(DateTime? fromDate, DateTime? toDate, string searchValue, string sortColumn, string sortColumnDirection)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var user = await UserService.GetAsync(x => x.Id == userId, ["Department"]);

            IQueryable<PersonalReminder> PersonalReminderQuery;
            PersonalReminderQuery = (IQueryable<PersonalReminder>)await BussinesService
                                 .GetAllAsync(w => !w.IsReminderCompleted && w.CreatedByUserId == user.Id
                                 , ["CreatedByUser"]);


            if (!string.IsNullOrEmpty(searchValue))
            {
                PersonalReminderQuery = PersonalReminderQuery.Where(b =>
                 (string.IsNullOrEmpty(b.Title) || b.Title.Contains(searchValue!))
                || (string.IsNullOrEmpty(b.Notes) || b.Notes.Contains(searchValue!))
                || (string.IsNullOrEmpty(b.CreatedByUser.FullName) || b.CreatedByUser.FullName.Contains(searchValue!))
                );
            }

            var PersonalReminder = PersonalReminderQuery.ToList();

            if (fromDate.HasValue)
                PersonalReminder = PersonalReminder.Where(x => x.DateCreated >= fromDate.Value).ToList();
            if (toDate.HasValue)
                PersonalReminder = PersonalReminder.Where(x => x.DateCreated <= toDate.Value).ToList();
            // Apply sorting in-memory based on known property names
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                switch (sortColumn)
                {
                    case "Title":
                        PersonalReminder = sortColumnDirection == "asc" ? PersonalReminder.OrderBy(b => b.Title).ToList() : PersonalReminder.OrderByDescending(b => b.Title).ToList();
                        break;
                    case "Notes":
                        PersonalReminder = sortColumnDirection == "asc" ? PersonalReminder.OrderBy(b => b.Notes).ToList() : PersonalReminder.OrderByDescending(b => b.Notes).ToList();
                        break;
                    case "CreatedByUserName":
                        PersonalReminder = sortColumnDirection == "asc" ? PersonalReminder.OrderBy(b => b.CreatedByUser.FullName).ToList() : PersonalReminder.OrderByDescending(b => b.CreatedByUser.FullName).ToList();
                        break;
                    case "ReminderDate":
                        PersonalReminder = sortColumnDirection == "asc" ? PersonalReminder.OrderBy(b => b.ReminderDate).ToList() : PersonalReminder.OrderByDescending(b => b.ReminderDate).ToList();
                        break;
                    case "IsReminderCompleted":
                        PersonalReminder = sortColumnDirection == "asc" ? PersonalReminder.OrderBy(b => b.IsReminderCompleted).ToList() : PersonalReminder.OrderByDescending(b => b.IsReminderCompleted).ToList();
                        break;
                    case "ReminderCompletedDate":
                        PersonalReminder = sortColumnDirection == "asc" ? PersonalReminder.OrderBy(b => b.ReminderCompletedDate).ToList() : PersonalReminder.OrderByDescending(b => b.ReminderCompletedDate).ToList();
                        break;
                    default:
                        PersonalReminder = PersonalReminder.OrderBy(b => b.Id).ToList(); // Default sorting
                        break;
                }
            }

            var recordsTotal = PersonalReminder.Count;
            PersonalReminder = PersonalReminder.ToList();
            var data = _mapper.Map<List<PersonalReminderViewModel>>(PersonalReminder);

            // Create Excel file
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("التذكيرات"); // Arabic for "Date"

                worksheet.Cells[1, 1].Value = "التذكير";
                worksheet.Cells[1, 2].Value = "من اضاف التذكير";
                worksheet.Cells[1, 3].Value = "تاريخ التذكير";
                worksheet.Cells[1, 4].Value = "حاله الانجاز";
                worksheet.Cells[1, 5].Value = "تاريخ الانجاز";
                worksheet.Cells[1, 6].Value = "ملاحظه";

                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].Title;
                    worksheet.Cells[i + 2, 2].Value = data[i].CreatedByUserName;
                    worksheet.Cells[i + 2, 3].Value = data[i].ReminderDate;
                    worksheet.Cells[i + 2, 3].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[i + 2, 4].Value = data[i].IsReminderCompleted ? "نعم" : "لا";
                    worksheet.Cells[i + 2, 5].Value = data[i].ReminderCompletedDate;
                    worksheet.Cells[i + 2, 5].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[i + 2, 6].Value = data[i].Notes;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);

                string excelName = $"بيانات التذكيرات الشخصيه-{DateTime.Now:dd/MM/yyyy}.xlsx"; // Filename in Arabic

                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

    }

}
