using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using StaffWork.Api.Controllers;
using StaffWork.Core.Consts;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using StaffWork.Core.Paramaters;
using Hangfire;
using OfficeOpenXml;
using Microsoft.AspNetCore.Hosting;

namespace StaffWork.Web.Controllers
{
    public class TaskReminderController : ApiBaseController<TaskReminder>
    {
        public readonly IServicesBase<User> UserService;
        public readonly IServicesBase<Notification> NotificationService;
        public readonly IServicesBase<TaskFile> TaskFileService;
        public readonly IServicesBase<TaskReminderFile> TaskReminderFileService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public TaskReminderController(IServicesBase<TaskReminder> servicesBase, IMapper mapper, IServicesBase<User> userService, IServicesBase<TaskFile> taskFileService, IServicesBase<TaskReminderFile> taskReminderFileService, IWebHostEnvironment webHostEnvironment, IServicesBase<Notification> notificationService) : base(servicesBase, mapper)
        {
            UserService = userService;
            TaskFileService = taskFileService;
            TaskReminderFileService = taskReminderFileService;
            _webHostEnvironment = webHostEnvironment;
            NotificationService = notificationService;
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
            var viewModel = new TaskReminderFormViewModel
            {
                TaskModelId = taskModelId,
                ReminderDate = DateTime.UtcNow
            };
            return View("Form", viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Create(TaskReminderFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var TaskReminder = _mapper.Map<TaskReminder>(viewModel);
            TaskReminder.CreatedByUserId = GetAuthenticatedUser();
            // Handle Book images
            if (viewModel.TaskReminderFormFiles != null && viewModel.TaskReminderFormFiles.Count > 0)
            {
                foreach (var image in viewModel.TaskReminderFormFiles)
                {
                    if (image.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                        var filePath = "/files/TasksReminder";
                        if (!Directory.Exists($"{_webHostEnvironment.WebRootPath}{filePath}"))
                            Directory.CreateDirectory($"{_webHostEnvironment.WebRootPath}{filePath}");

                        var path = Path.Combine($"{_webHostEnvironment.WebRootPath}{filePath}", fileName);

                        using var stream = System.IO.File.Create(path);
                         image.CopyTo(stream);

                        var taskReminderFile = new TaskReminderFile
                        {
                            FileUrl = $"{filePath}/{fileName}",
                            FileName = image.FileName,
                            TaskReminder = TaskReminder
                        };
                        TaskReminder.TaskReminderFiles.Add(taskReminderFile);
                    }
                }
            }
            await BussinesService.InsertAsync(TaskReminder);
            #region Send Notification
            var reminder = await BussinesService.GetAsync(x => x.Id == TaskReminder.Id);
            var reminderDto = _mapper.Map<TaskReminderViewModel>(reminder);
            var scheduleTime = reminder.ReminderDate.AddDays(-1) - DateTime.Now;
            if (scheduleTime.TotalSeconds > 0) // Ensure scheduling in the future
            {
                reminder.JobId = BackgroundJob.Schedule<NotifiJob>(
                    x => x.CheckTaskReminderDates(reminderDto),
                    scheduleTime
                );
            }
            else
            {
                // If the EndDate is in less than a day, schedule immediately
                reminder.JobId = BackgroundJob.Enqueue<NotifiJob>(x => x.CheckTaskReminderDates(reminderDto));
            }
            #endregion
            return RedirectToAction("Index", _mapper.Map<TaskReminderViewModel>(TaskReminder));
        }
        [HttpGet]
        public async Task<IActionResult> EditAsync(int id)
        {
            var TaskReminder = await BussinesService.GetAsync(d => d.Id == id, ["TaskReminderFiles", "TaskModel.AssignedUsers", "TaskModel.TaskFiles", "TaskModel.AssignedUsers.User"]);
            if (TaskReminder == null)
                return NotFound();
            var viewModel = _mapper.Map<TaskReminderFormViewModel>(TaskReminder);
            viewModel.ExistingFiles = _mapper.Map<List<TaskFileDisplay>>(TaskReminder.TaskReminderFiles);
            return View("Form", PopulateViewModel(viewModel));
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditAsync(TaskReminderFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var TaskReminder = await BussinesService.GetAsync(d => d.Id == viewModel.Id);
            if (TaskReminder == null)
                return NotFound();
            _mapper.Map(viewModel, TaskReminder);
            #region Files List
            // Handle file uploads
            if (viewModel.TaskReminderFormFiles.Any())
            {
                foreach (var file in viewModel.TaskReminderFormFiles)
                {
                    if (file.Length > 0)
                    {

                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var filePath = "/files/TasksReminder";
                        if (!Directory.Exists($"{_webHostEnvironment.WebRootPath}{filePath}"))
                            Directory.CreateDirectory($"{_webHostEnvironment.WebRootPath}{filePath}");

                        var path = Path.Combine($"{_webHostEnvironment.WebRootPath}{filePath}", fileName);

                        using var stream = System.IO.File.Create(path);
                        await file.CopyToAsync(stream);

                        var taskReminderFile = new TaskReminderFile
                        {
                            FileUrl = $"{filePath}/{fileName}",
                            FileName = file.FileName,
                            TaskReminder = TaskReminder
                        };
                        TaskReminder.TaskReminderFiles.Add(taskReminderFile);
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
            await BussinesService.UpdateAsync(TaskReminder.Id, TaskReminder);

            #region Send Notification
            var reminder = await BussinesService.GetAsync(x => x.Id == TaskReminder.Id);
            var reminderDto = _mapper.Map<TaskReminderViewModel>(reminder);
            if (reminder.JobId != null)
            {
                BackgroundJob.Delete(reminder.JobId);
            }
            if (!reminder.IsReminderCompleted)
            {
                var scheduleTime = reminder.ReminderDate.AddDays(-1) - DateTime.Now;
                if (scheduleTime.TotalSeconds > 0) // Ensure scheduling in the future
                {
                    reminder.JobId = BackgroundJob.Schedule<NotifiJob>(
                        x => x.CheckTaskReminderDates(reminderDto),
                        scheduleTime
                    );
                }
                else
                {
                    // If the EndDate is in less than a day, schedule immediately
                    reminder.JobId = BackgroundJob.Enqueue<NotifiJob>(x => x.CheckTaskReminderDates(reminderDto));
                }
            }
            #endregion
            return RedirectToAction("Index", _mapper.Map<TaskReminderViewModel>(TaskReminder));
        }
       // [Authorize(Roles = AppRoles.Admin + "," + AppRoles.SuperAdmin)]
        public async Task<IActionResult> Delete(int id)
        {
            var TaskReminder = await BussinesService.GetAsync(d => d.Id == id, ["TaskReminderFiles", "Notifications", "TaskModel", "TaskModel.AssignedUsers", "TaskModel.TaskFiles", "TaskModel.AssignedUsers.User"]);
            if (TaskReminder == null)
                return NotFound();
            try
            {
                if (TaskReminder.JobId != null)
                {
                    BackgroundJob.Delete(TaskReminder.JobId);
                }
                foreach (var item in TaskReminder.Notifications.ToList())
                {
                    await NotificationService.DeleteAsync(item.Id);
                }
                foreach (var item in TaskReminder.TaskReminderFiles.ToList())
                {
                    await TaskReminderFileService.DeleteAsync(item.Id);
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
            var TaskReminder = await BussinesService.GetAsync(d => d.Id == Id, ["TaskModel", "TaskModel.AssignedUsers", "TaskModel.TaskFiles", "TaskModel.AssignedUsers.User"]);
            if (TaskReminder == null)
                return NotFound();
            try
            {
                if (TaskReminder.IsReminderCompleted)
                {
                    return BadRequest(new { Message = "This item is already completed." });
                }

                if (!TaskReminder.TaskModel.AssignedUsers.Any(x => x.UserId == currentUserId))
                {
                    return Forbid("you're not authorized");
                }
                TaskReminder.IsReminderCompleted = true;
                TaskReminder.ReminderCompletedDate = DateTime.UtcNow;
                if (TaskReminder.JobId != null)
                {
                    BackgroundJob.Delete(TaskReminder.JobId);
                }
                await BussinesService.UpdateAsync(TaskReminder.Id, TaskReminder);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }
        private TaskReminderFormViewModel PopulateViewModel(TaskReminderFormViewModel? model = null)
        {
            var userId = GetAuthenticatedUser();
            var user = UserService.GetAsync(x => x.Id == userId, ["Department"]).Result;

            TaskReminderFormViewModel viewModel = model is null ? new TaskReminderFormViewModel() : model;

            //List<User> users = [];
            //if (User.IsInRole(AppRoles.SuperAdmin))
            //    users = UserService.GetAllAsync(null!, ["Department"]).Result.ToList();
            //else if (User.IsInRole(AppRoles.Admin))
            //    users = UserService.GetAllAsync(x => x.DepartmentId == user.DepartmentId, ["Department"]).Result.ToList();
            //else
            //    users = UserService.GetAllAsync(x => x.Id == user.Id, ["Department"]).Result.ToList();

            //viewModel.Users = _mapper.Map<IEnumerable<SelectListItem>>(users);

            return viewModel;
        }

        [HttpPost]
        public async Task<IActionResult> GetTaskReminders(DateTime? fromDate, DateTime? toDate)
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

            IQueryable<TaskReminder> TaskReminderQuery;

            if (User.IsInRole(AppRoles.SuperAdmin))
                TaskReminderQuery = (IQueryable<TaskReminder>)await BussinesService.GetAllAsync(null!, ["CreatedByUser", "TaskModel", "TaskModel.AssignedUsers", "TaskModel.AssignedUsers.User"]);
            else if (User.IsInRole(AppRoles.Admin))
                TaskReminderQuery = (IQueryable<TaskReminder>)await BussinesService
                    .GetAllAsync(w => w.TaskModel.AssignedUsers.Any(x => x.User.DepartmentId == user.DepartmentId)
                    , ["CreatedByUser", "TaskModel", "TaskModel.AssignedUsers", "TaskModel.AssignedUsers.User"]);
            else
                TaskReminderQuery = (IQueryable<TaskReminder>)await BussinesService
                                   .GetAllAsync(w => w.TaskModel.AssignedUsers.Any(x => x.UserId == user.Id)
                                   , ["CreatedByUser", "TaskModel", "TaskModel.AssignedUsers", "TaskModel.AssignedUsers.User"]);

            if (!string.IsNullOrEmpty(searchValue))
            {
                TaskReminderQuery = TaskReminderQuery.Where(b => b.TaskModel.AssignedUsers.Any(x => x.User.FullName.Contains(searchValue!))
                || (string.IsNullOrEmpty(b.Title) || b.Title.Contains(searchValue!))
                || (string.IsNullOrEmpty(b.Notes) || b.Notes.Contains(searchValue!))
                || (string.IsNullOrEmpty(b.TaskModel.Title) || b.TaskModel.Title.Contains(searchValue!))
                || (string.IsNullOrEmpty(b.CreatedByUser.FullName) || b.CreatedByUser.FullName.Contains(searchValue!))
                );
            }

            var TaskReminder = TaskReminderQuery.ToList();

            if (fromDate.HasValue)
                TaskReminder = TaskReminder.Where(x => x.DateCreated >= fromDate.Value).ToList();
            if (toDate.HasValue)
                TaskReminder = TaskReminder.Where(x => x.DateCreated <= toDate.Value).ToList();
            // Apply sorting in-memory based on known property names
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                switch (sortColumn)
                {
                    case "Title":
                        TaskReminder = sortColumnDirection == "asc" ? TaskReminder.OrderBy(b => b.Title).ToList() : TaskReminder.OrderByDescending(b => b.Title).ToList();
                        break;
                    case "Notes":
                        TaskReminder = sortColumnDirection == "asc" ? TaskReminder.OrderBy(b => b.Notes).ToList() : TaskReminder.OrderByDescending(b => b.Notes).ToList();
                        break;
                    case "TaskModelTitle":
                        TaskReminder = sortColumnDirection == "asc" ? TaskReminder.OrderBy(b => b.TaskModel.Title).ToList() : TaskReminder.OrderByDescending(b => b.TaskModel.Title).ToList();
                        break;
                    case "CreatedByUserName":
                        TaskReminder = sortColumnDirection == "asc" ? TaskReminder.OrderBy(b => b.CreatedByUser.FullName).ToList() : TaskReminder.OrderByDescending(b => b.CreatedByUser.FullName).ToList();
                        break;
                    case "ReminderDate":
                        TaskReminder = sortColumnDirection == "asc" ? TaskReminder.OrderBy(b => b.ReminderDate).ToList() : TaskReminder.OrderByDescending(b => b.ReminderDate).ToList();
                        break;
                    case "IsReminderCompleted":
                        TaskReminder = sortColumnDirection == "asc" ? TaskReminder.OrderBy(b => b.IsReminderCompleted).ToList() : TaskReminder.OrderByDescending(b => b.IsReminderCompleted).ToList();
                        break;
                    case "ReminderCompletedDate":
                        TaskReminder = sortColumnDirection == "asc" ? TaskReminder.OrderBy(b => b.ReminderCompletedDate).ToList() : TaskReminder.OrderByDescending(b => b.ReminderCompletedDate).ToList();
                        break;
                    default:
                        TaskReminder = TaskReminder.OrderBy(b => b.Id).ToList(); // Default sorting
                        break;
                }
            }
            var recordsTotal = TaskReminder.Count;
            TaskReminder = TaskReminder.ToList();
            var data = TaskReminder.Skip(skip).Take(pageSize).ToList();

            var mappedData = _mapper.Map<IEnumerable<TaskReminderViewModel>>(TaskReminder);


            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = mappedData };

            return Ok(jsonData);
        }

        public async Task<IActionResult> ExportToExcelAsync(DateTime? fromDate, DateTime? toDate, string searchValue, string sortColumn, string sortColumnDirection)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var user = await UserService.GetAsync(x => x.Id == userId, ["Department"]);

            IQueryable<TaskReminder> TaskReminderQuery;

            if (User.IsInRole(AppRoles.SuperAdmin))
                TaskReminderQuery = (IQueryable<TaskReminder>)await BussinesService.GetAllAsync(null!, ["CreatedByUser", "TaskModel", "TaskModel.AssignedUsers", "TaskModel.AssignedUsers.User"]);
            else if (User.IsInRole(AppRoles.Admin))
                TaskReminderQuery = (IQueryable<TaskReminder>)await BussinesService
                    .GetAllAsync(w => w.TaskModel.AssignedUsers.Any(x => x.User.DepartmentId == user.DepartmentId)
                    , ["CreatedByUser", "AssignedUsers", "Reminders", "AssignedUsers.User"]);
            else
                TaskReminderQuery = (IQueryable<TaskReminder>)await BussinesService
                                   .GetAllAsync(w => w.TaskModel.AssignedUsers.Any(x => x.UserId == user.Id)
                                   , ["CreatedByUser", "TaskModel", "TaskModel.AssignedUsers", "TaskModel.AssignedUsers.User"]);

            if (!string.IsNullOrEmpty(searchValue))
            {
                TaskReminderQuery = TaskReminderQuery.Where(b => b.TaskModel.AssignedUsers.Any(x => x.User.FullName.Contains(searchValue!))
                || (string.IsNullOrEmpty(b.Title) || b.Title.Contains(searchValue!))
                || (string.IsNullOrEmpty(b.Notes) || b.Notes.Contains(searchValue!))
                || (string.IsNullOrEmpty(b.TaskModel.Title) || b.TaskModel.Title.Contains(searchValue!))
                || (string.IsNullOrEmpty(b.CreatedByUser.FullName) || b.CreatedByUser.FullName.Contains(searchValue!))
                );
            }

            var TaskReminder = TaskReminderQuery.ToList();

            if (fromDate.HasValue)
                TaskReminder = TaskReminder.Where(x => x.DateCreated >= fromDate.Value).ToList();
            if (toDate.HasValue)
                TaskReminder = TaskReminder.Where(x => x.DateCreated <= toDate.Value).ToList();
            // Apply sorting in-memory based on known property names
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                switch (sortColumn)
                {
                    case "Title":
                        TaskReminder = sortColumnDirection == "asc" ? TaskReminder.OrderBy(b => b.Title).ToList() : TaskReminder.OrderByDescending(b => b.Title).ToList();
                        break;
                    case "Notes":
                        TaskReminder = sortColumnDirection == "asc" ? TaskReminder.OrderBy(b => b.Notes).ToList() : TaskReminder.OrderByDescending(b => b.Notes).ToList();
                        break;
                    case "TaskModelTitle":
                        TaskReminder = sortColumnDirection == "asc" ? TaskReminder.OrderBy(b => b.TaskModel.Title).ToList() : TaskReminder.OrderByDescending(b => b.TaskModel.Title).ToList();
                        break;
                    case "CreatedByUserName":
                        TaskReminder = sortColumnDirection == "asc" ? TaskReminder.OrderBy(b => b.CreatedByUser.FullName).ToList() : TaskReminder.OrderByDescending(b => b.CreatedByUser.FullName).ToList();
                        break;
                    case "ReminderDate":
                        TaskReminder = sortColumnDirection == "asc" ? TaskReminder.OrderBy(b => b.ReminderDate).ToList() : TaskReminder.OrderByDescending(b => b.ReminderDate).ToList();
                        break;
                    case "IsReminderCompleted":
                        TaskReminder = sortColumnDirection == "asc" ? TaskReminder.OrderBy(b => b.IsReminderCompleted).ToList() : TaskReminder.OrderByDescending(b => b.IsReminderCompleted).ToList();
                        break;
                    case "ReminderCompletedDate":
                        TaskReminder = sortColumnDirection == "asc" ? TaskReminder.OrderBy(b => b.ReminderCompletedDate).ToList() : TaskReminder.OrderByDescending(b => b.ReminderCompletedDate).ToList();
                        break;
                    default:
                        TaskReminder = TaskReminder.OrderBy(b => b.Id).ToList(); // Default sorting
                        break;
                }
            }

            var recordsTotal = TaskReminder.Count;
            TaskReminder = TaskReminder.ToList();
            var data = _mapper.Map<List<TaskReminderViewModel>>(TaskReminder);

            // Create Excel file
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("التذكيرات"); // Arabic for "Date"

                // Manually set the Arabic headers
                worksheet.Cells[1, 1].Value = "التذكير";
                worksheet.Cells[1, 2].Value = "المهمه";
                worksheet.Cells[1, 3].Value = "من اضاف التذكير";
                worksheet.Cells[1, 4].Value = "تاريخ التذكير";
                worksheet.Cells[1, 5].Value = "حاله الانجاز";
                worksheet.Cells[1, 6].Value = "تاريخ الانجاز";
                worksheet.Cells[1, 7].Value = "ملاحظه";

                // Load data starting from row 2 (after the headers)
                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].Title;
                    worksheet.Cells[i + 2, 2].Value = data[i].TaskModelTitle;
                    worksheet.Cells[i + 2, 3].Value = data[i].CreatedByUserName;
                    worksheet.Cells[i + 2, 4].Value = data[i].ReminderDate;
                    worksheet.Cells[i + 2, 4].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[i + 2, 5].Value = data[i].IsReminderCompleted ? "نعم" : "لا";
                    worksheet.Cells[i + 2, 6].Value = data[i].ReminderCompletedDate;
                    worksheet.Cells[i + 2, 6].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[i + 2, 7].Value = data[i].Notes;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);

                string excelName = $"بيانات التذكيرات-{DateTime.Now:dd/MM/yyyy}.xlsx"; // Filename in Arabic

                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

    }

}
