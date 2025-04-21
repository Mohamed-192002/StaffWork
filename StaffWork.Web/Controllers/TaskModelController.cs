using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using StaffWork.Api.Controllers;
using StaffWork.Core.Consts;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using StaffWork.Core.Paramaters;

namespace StaffWork.Web.Controllers
{
    public class TaskModelController : ApiBaseController<TaskModel>
    {
        public readonly IServicesBase<User> UserService;
        public readonly IServicesBase<TaskFile> TaskFileService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public TaskModelController(IServicesBase<TaskModel> servicesBase, IMapper mapper, IServicesBase<User> userService, IWebHostEnvironment webHostEnvironment, IServicesBase<TaskFile> taskFileService) : base(servicesBase, mapper)
        {
            UserService = userService;
            _webHostEnvironment = webHostEnvironment;
            TaskFileService = taskFileService;
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
        [Authorize(Roles = AppRoles.Admin + "," + AppRoles.SuperAdmin)]
        public IActionResult Create()
        {
            return View("Form", PopulateViewModel());
        }
        [HttpPost]
        [Authorize(Roles = AppRoles.Admin + "," + AppRoles.SuperAdmin)]
        public async Task<IActionResult> Create(TaskModelFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var TaskModel = _mapper.Map<TaskModel>(viewModel);
            foreach (var userId in viewModel.SelectedUsers)
            {
                var user = await UserService.GetAsync(x => x.Id == userId);
                if (user != null)
                {
                    TaskModel.AssignedUsers.Add(new TaskUser
                    {
                        UserId = user.Id,
                        TaskModel = TaskModel
                    });
                }
            }
            // Handle Book images
            if (viewModel.TaskFiles != null && viewModel.TaskFiles.Count > 0)
            {
                foreach (var image in viewModel.TaskFiles)
                {
                    if (image.Length > 0)
                    {
                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
                        var filePath = "/files/Tasks";
                        if (!Directory.Exists($"{_webHostEnvironment.WebRootPath}{filePath}"))
                            Directory.CreateDirectory($"{_webHostEnvironment.WebRootPath}{filePath}");

                        var path = Path.Combine($"{_webHostEnvironment.WebRootPath}{filePath}", fileName);

                        using var stream = System.IO.File.Create(path);
                        await image.CopyToAsync(stream);

                        var taskFile = new TaskFile
                        {
                            FileUrl = $"{filePath}/{fileName}",
                            FileName = image.FileName,
                            TaskModel = TaskModel
                        };
                    }
                }
            }
            await BussinesService.InsertAsync(TaskModel);
            return RedirectToAction("Index", _mapper.Map<TaskModelViewModel>(TaskModel));
        }
        [HttpGet]
        [Authorize(Roles = AppRoles.Admin + "," + AppRoles.SuperAdmin)]
        public async Task<IActionResult> EditAsync(int id)
        {
            var TaskModel = await BussinesService.GetAsync(d => d.Id == id, ["AssignedUsers", "AssignedUsers.User", "TaskFiles"]);
            if (TaskModel == null)
                return NotFound();
            var viewModel = _mapper.Map<TaskModelFormViewModel>(TaskModel);
            viewModel.SelectedUsers = TaskModel.AssignedUsers.Select(x => x.UserId).ToList();
            viewModel.ExistingFiles = _mapper.Map<List<TaskFileDisplay>>(TaskModel.TaskFiles);
            return View("Form", PopulateViewModel(viewModel));
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = AppRoles.Admin + "," + AppRoles.SuperAdmin)]
        public async Task<IActionResult> EditAsync(TaskModelFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var TaskModel = await BussinesService.GetAsync(d => d.Id == viewModel.Id, ["AssignedUsers", "Reminders", "AssignedUsers.User"]);
            if (TaskModel == null)
                return NotFound();
            _mapper.Map(viewModel, TaskModel);

            // Clear existing assigned users
            TaskModel.AssignedUsers.Clear();
            foreach (var userId in viewModel.SelectedUsers)
            {
                var user = await UserService.GetAsync(x => x.Id == userId);
                if (user != null)
                {
                    TaskModel.AssignedUsers.Add(new TaskUser
                    {
                        UserId = user.Id,
                        TaskModel = TaskModel
                    });
                }
            }


            #region Files List
            // Handle file uploads
            if (viewModel.TaskFiles.Any())
            {
                foreach (var file in viewModel.TaskFiles)
                {
                    if (file.Length > 0)
                    {

                        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                        var filePath = "/files/Tasks";
                        if (!Directory.Exists($"{_webHostEnvironment.WebRootPath}{filePath}"))
                            Directory.CreateDirectory($"{_webHostEnvironment.WebRootPath}{filePath}");

                        var path = Path.Combine($"{_webHostEnvironment.WebRootPath}{filePath}", fileName);

                        using var stream = System.IO.File.Create(path);
                        await file.CopyToAsync(stream);

                        var taskFile = new TaskFile
                        {
                            FileUrl = $"{filePath}/{fileName}",
                            FileName = file.FileName,
                            TaskModel = TaskModel
                        };
                        TaskModel.TaskFiles.Add(taskFile);
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
            await BussinesService.UpdateAsync(TaskModel.Id, TaskModel);
            return RedirectToAction("Index", _mapper.Map<TaskModelViewModel>(TaskModel));
        }
        [Authorize(Roles = AppRoles.Admin + "," + AppRoles.SuperAdmin)]
        public async Task<IActionResult> Delete(int id)
        {
            var TaskModel = await BussinesService.GetAsync(d => d.Id == id, ["AssignedUsers", "TaskFiles", "Reminders", "AssignedUsers.User"]);
            if (TaskModel == null)
                return NotFound();
            try
            {
                TaskModel.AssignedUsers.Clear();
                await BussinesService.DeleteAsync(id);
            }
            catch (Exception)
            {
                throw;
            }
            return Ok();
        }
        public async Task<IActionResult> Complete(int id)
        {
            var currentUserId = GetAuthenticatedUser();
            var TaskModel = await BussinesService.GetAsync(d => d.Id == id, ["AssignedUsers", "TaskFiles", "Reminders", "AssignedUsers.User"]);
            if (TaskModel == null)
                return NotFound();
            try
            {
                if (TaskModel.IsCompleted)
                {
                    return BadRequest(new { Message = "This item is already completed." });
                }

                if (!(User.IsInRole(AppRoles.SuperAdmin) || !(User.IsInRole(AppRoles.Admin)) || !TaskModel.AssignedUsers.Any(x => x.UserId == currentUserId)))
                {
                    return Forbid("you're not authorized");
                }
                TaskModel.IsCompleted = true;
                TaskModel.DateCompleted = DateTime.UtcNow;
                await BussinesService.UpdateAsync(TaskModel.Id, TaskModel);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IActionResult> Received(int id)
        {
            var currentUserId = GetAuthenticatedUser();
            var TaskModel = await BussinesService.GetAsync(d => d.Id == id, ["AssignedUsers", "TaskFiles", "Reminders", "AssignedUsers.User"]);
            if (TaskModel == null)
                return NotFound();
            try
            {
                if (TaskModel.IsReceived)
                {
                    return BadRequest(new { Message = "This item is already Received." });
                }

                if (!(User.IsInRole(AppRoles.SuperAdmin) || !(User.IsInRole(AppRoles.Admin)) || !TaskModel.AssignedUsers.Any(x => x.UserId == currentUserId)))
                {
                    return Forbid("you're not authorized");
                }
                TaskModel.IsReceived = true;
                TaskModel.DateReceived = DateTime.UtcNow;
                TaskModel.ReceivedByUserId = currentUserId;
                await BussinesService.UpdateAsync(TaskModel.Id, TaskModel);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var currentUserId = GetAuthenticatedUser();
            var TaskModel = BussinesService.GetAsync(d => d.Id == id, ["AssignedUsers", "TaskFiles", "Reminders", "Reminders.CreatedByUser", "AssignedUsers.User"]).Result;
            if (TaskModel == null)
                return NotFound();
            var viewmodel = _mapper.Map<TaskModelViewModel>(TaskModel);
            viewmodel.Reminders = viewmodel.Reminders
                    .OrderByDescending(x => x.ReminderDate)
                    .ToList();
            viewmodel.ExistingFiles = _mapper.Map<List<TaskFileDisplay>>(TaskModel.TaskFiles);
            return View(viewmodel);
        }
        private TaskModelFormViewModel PopulateViewModel(TaskModelFormViewModel? model = null)
        {
            var userId = GetAuthenticatedUser();
            var user = UserService.GetAsync(x => x.Id == userId, ["Department"]).Result;

            TaskModelFormViewModel viewModel = model is null ? new TaskModelFormViewModel() : model;

            List<User> users = [];
            if (User.IsInRole(AppRoles.SuperAdmin))
                users = UserService.GetAllAsync(null!, ["Department"]).Result.ToList();
            else if (User.IsInRole(AppRoles.Admin))
                users = UserService.GetAllAsync(x => x.DepartmentId == user.DepartmentId, ["Department"]).Result.ToList();
            else
                users = UserService.GetAllAsync(x => x.Id == user.Id, ["Department"]).Result.ToList();

            viewModel.Users = _mapper.Map<IEnumerable<SelectListItem>>(users);

            return viewModel;
        }

        [HttpPost]
        public async Task<IActionResult> GetTaskModels(DateTime? fromDate, DateTime? toDate)
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

            IQueryable<TaskModel> TaskModelQuery;

            if (User.IsInRole(AppRoles.SuperAdmin))
                TaskModelQuery = (IQueryable<TaskModel>)await BussinesService.GetAllAsync(null!, ["AssignedUsers", "Reminders", "AssignedUsers.User"]);
            else if (User.IsInRole(AppRoles.Admin))
                TaskModelQuery = (IQueryable<TaskModel>)await BussinesService
                    .GetAllAsync(w => w.AssignedUsers.Any(x => x.User.DepartmentId == user.DepartmentId)
                    , ["AssignedUsers", "Reminders", "AssignedUsers.User"]);
            else
                TaskModelQuery = (IQueryable<TaskModel>)await BussinesService
                                   .GetAllAsync(w => w.AssignedUsers.Any(x => x.UserId == user.Id)
                                   , ["AssignedUsers", "Reminders", "AssignedUsers.User"]);

            if (!string.IsNullOrEmpty(searchValue))
            {
                TaskModelQuery = TaskModelQuery.Where(b =>
                b.AssignedUsers.Any(x => x.User.FullName.Contains(searchValue!))
               || (string.IsNullOrEmpty(b.Title) || b.Title.Contains(searchValue!))
                || (string.IsNullOrEmpty(b.Notes) || b.Notes.Contains(searchValue!))
                );
            }

            var TaskModel = TaskModelQuery.ToList();

            if (fromDate.HasValue)
                TaskModel = TaskModel.Where(x => x.DateCreated >= fromDate.Value).ToList();
            if (toDate.HasValue)
                TaskModel = TaskModel.Where(x => x.DateCreated <= toDate.Value).ToList();

            // Apply sorting in-memory based on known property names
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                switch (sortColumn)
                {
                    case "Title":
                        TaskModel = sortColumnDirection == "asc" ? TaskModel.OrderBy(b => b.Title).ToList() : TaskModel.OrderByDescending(b => b.Title).ToList();
                        break;
                    case "Notes":
                        TaskModel = sortColumnDirection == "asc" ? TaskModel.OrderBy(b => b.Notes).ToList() : TaskModel.OrderByDescending(b => b.Notes).ToList();
                        break;
                    case "IsReceived":
                        TaskModel = sortColumnDirection == "asc" ? TaskModel.OrderBy(b => b.IsReceived).ToList() : TaskModel.OrderByDescending(b => b.IsReceived).ToList();
                        break;
                    case "DateReceived":
                        TaskModel = sortColumnDirection == "asc" ? TaskModel.OrderBy(b => b.DateReceived).ToList() : TaskModel.OrderByDescending(b => b.DateReceived).ToList();
                        break;
                    case "IsCompleted":
                        TaskModel = sortColumnDirection == "asc" ? TaskModel.OrderBy(b => b.IsCompleted).ToList() : TaskModel.OrderByDescending(b => b.IsCompleted).ToList();
                        break;
                    case "DateCompleted":
                        TaskModel = sortColumnDirection == "asc" ? TaskModel.OrderBy(b => b.DateCompleted).ToList() : TaskModel.OrderByDescending(b => b.DateCompleted).ToList();
                        break;
                    default:
                        TaskModel = TaskModel.OrderBy(b => b.Id).ToList(); // Default sorting
                        break;
                }
            }

            var recordsTotal = TaskModel.Count;
            TaskModel = TaskModel.ToList();
            var data = TaskModel.Skip(skip).Take(pageSize).ToList();

            var mappedData = _mapper.Map<IEnumerable<TaskModelViewModel>>(TaskModel);


            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = mappedData };

            return Ok(jsonData);
        }

        public async Task<IActionResult> ExportToExcelAsync(DateTime? fromDate, DateTime? toDate, string searchValue, string sortColumn, string sortColumnDirection)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var user = await UserService.GetAsync(x => x.Id == userId, ["Department"]);

            IQueryable<TaskModel> TaskModelQuery;

            if (User.IsInRole(AppRoles.SuperAdmin))
                TaskModelQuery = (IQueryable<TaskModel>)await BussinesService.GetAllAsync(null!, ["AssignedUsers", "Reminders", "AssignedUsers.User"]);
            else if (User.IsInRole(AppRoles.Admin))
                TaskModelQuery = (IQueryable<TaskModel>)await BussinesService
                    .GetAllAsync(w => w.AssignedUsers.Any(x => x.User.DepartmentId == user.DepartmentId)
                    , ["AssignedUsers", "Reminders", "AssignedUsers.User"]);
            else
                TaskModelQuery = (IQueryable<TaskModel>)await BussinesService
                                   .GetAllAsync(w => w.AssignedUsers.Any(x => x.UserId == user.Id)
                                   , ["AssignedUsers", "Reminders", "AssignedUsers.User"]);

            if (!string.IsNullOrEmpty(searchValue))
            {
                TaskModelQuery = TaskModelQuery.Where(b =>
                b.AssignedUsers.Any(x => x.User.FullName.Contains(searchValue!))
               || (string.IsNullOrEmpty(b.Title) || b.Title.Contains(searchValue!))
                || (string.IsNullOrEmpty(b.Notes) || b.Notes.Contains(searchValue!))
                );
            }

            var TaskModel = TaskModelQuery.ToList();

            if (fromDate.HasValue)
                TaskModel = TaskModel.Where(x => x.DateCreated >= fromDate.Value).ToList();
            if (toDate.HasValue)
                TaskModel = TaskModel.Where(x => x.DateCreated <= toDate.Value).ToList();

            // Apply sorting in-memory based on known property names
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                switch (sortColumn)
                {
                    case "Title":
                        TaskModel = sortColumnDirection == "asc" ? TaskModel.OrderBy(b => b.Title).ToList() : TaskModel.OrderByDescending(b => b.Title).ToList();
                        break;
                    case "Notes":
                        TaskModel = sortColumnDirection == "asc" ? TaskModel.OrderBy(b => b.Notes).ToList() : TaskModel.OrderByDescending(b => b.Notes).ToList();
                        break;
                    case "IsReceived":
                        TaskModel = sortColumnDirection == "asc" ? TaskModel.OrderBy(b => b.IsReceived).ToList() : TaskModel.OrderByDescending(b => b.IsReceived).ToList();
                        break;
                    case "DateReceived":
                        TaskModel = sortColumnDirection == "asc" ? TaskModel.OrderBy(b => b.DateReceived).ToList() : TaskModel.OrderByDescending(b => b.DateReceived).ToList();
                        break;
                    case "IsCompleted":
                        TaskModel = sortColumnDirection == "asc" ? TaskModel.OrderBy(b => b.IsCompleted).ToList() : TaskModel.OrderByDescending(b => b.IsCompleted).ToList();
                        break;
                    case "DateCompleted":
                        TaskModel = sortColumnDirection == "asc" ? TaskModel.OrderBy(b => b.DateCompleted).ToList() : TaskModel.OrderByDescending(b => b.DateCompleted).ToList();
                        break;
                    default:
                        TaskModel = TaskModel.OrderBy(b => b.Id).ToList(); // Default sorting
                        break;
                }
            }

            var recordsTotal = TaskModel.Count;
            TaskModel = TaskModel.ToList();
            var data = _mapper.Map<List<TaskModelViewModel>>(TaskModel);

            // Create Excel file
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("المهام"); // Arabic for "Date"

                // Manually set the Arabic headers
                worksheet.Cells[1, 1].Value = "اسم المهمه";
                worksheet.Cells[1, 2].Value = "الموظفين";
                worksheet.Cells[1, 3].Value = "حاله الاستلام";
                worksheet.Cells[1, 4].Value = "تاريخ الاستلام";
                worksheet.Cells[1, 5].Value = "حاله الانجاز";
                worksheet.Cells[1, 6].Value = "تاريخ الانجاز";
                worksheet.Cells[1, 7].Value = "ملاحظه";

                // Load data starting from row 2 (after the headers)
                for (int i = 0; i < data.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = data[i].Title;
                    worksheet.Cells[i + 2, 2].Value = string.Join(", ", data[i].Users);
                    worksheet.Cells[i + 2, 3].Value = data[i].IsReceived ? "نعم" : "لا";
                    worksheet.Cells[i + 2, 4].Value = data[i].DateReceived;
                    worksheet.Cells[i + 2, 4].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[i + 2, 5].Value = data[i].IsCompleted ? "نعم" : "لا";
                    worksheet.Cells[i + 2, 6].Value = data[i].DateCompleted;
                    worksheet.Cells[i + 2, 6].Style.Numberformat.Format = "dd/MM/yyyy";
                    worksheet.Cells[i + 2, 7].Value = data[i].Notes;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);

                string excelName = $"بيانات المهام-{DateTime.Now:dd/MM/yyyy}.xlsx"; // Filename in Arabic

                stream.Position = 0;
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
        }

    }

}
