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

namespace StaffWork.Web.Controllers
{
    public class TaskReminderController : ApiBaseController<TaskReminder>
    {
        public readonly IServicesBase<User> UserService;
        public readonly IServicesBase<TaskFile> TaskFileService;

        public TaskReminderController(IServicesBase<TaskReminder> servicesBase, IMapper mapper, IServicesBase<User> userService, IServicesBase<TaskFile> taskFileService) : base(servicesBase, mapper)
        {
            UserService = userService;
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
        public IActionResult Create()
        {
            return View("Form", PopulateViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Create(TaskReminderFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var TaskReminder = _mapper.Map<TaskReminder>(viewModel);

            await BussinesService.InsertAsync(TaskReminder);
            return RedirectToAction("Index", _mapper.Map<TaskReminderViewModel>(TaskReminder));
        }
        [HttpGet]
        public async Task<IActionResult> EditAsync(int id)
        {
            var TaskReminder = await BussinesService.GetAsync(d => d.Id == id, ["TaskModel.AssignedUsers", "TaskModel.TaskFiles", "TaskModel.AssignedUsers.User"]);
            if (TaskReminder == null)
                return NotFound();
            var viewModel = _mapper.Map<TaskReminderFormViewModel>(TaskReminder);
            return View("Form", PopulateViewModel(viewModel));
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditAsync(TaskReminderFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var TaskReminder = await BussinesService.GetAsync(d => d.Id == viewModel.Id, ["TaskModel.AssignedUsers", "TaskModel.TaskFiles", "TaskModel.AssignedUsers.User"]);
            if (TaskReminder == null)
                return NotFound();
            _mapper.Map(viewModel, TaskReminder);

            await BussinesService.UpdateAsync(TaskReminder.Id, TaskReminder);
            return RedirectToAction("Index", _mapper.Map<TaskReminderViewModel>(TaskReminder));
        }
        [Authorize(Roles = AppRoles.Admin + "," + AppRoles.SuperAdmin)]
        public async Task<IActionResult> Delete(int id)
        {
            var TaskReminder = await BussinesService.GetAsync(d => d.Id == id, ["TaskModel.AssignedUsers", "TaskModel.TaskFiles", "TaskModel.AssignedUsers.User"]);
            if (TaskReminder == null)
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
        public async Task<IActionResult> Complete(int id)
        {
            var currentUserId = GetAuthenticatedUser();
            var TaskReminder = await BussinesService.GetAsync(d => d.Id == id, ["TaskModel.AssignedUsers", "TaskModel.TaskFiles", "TaskModel.AssignedUsers.User"]);
            if (TaskReminder == null)
                return NotFound();
            try
            {
                if (TaskReminder.IsReminderCompleted)
                {
                    return BadRequest(new { Message = "This item is already completed." });
                }

                if (!(User.IsInRole(AppRoles.SuperAdmin) || !TaskReminder.TaskModel.AssignedUsers.Any(x => x.UserId == currentUserId)))
                {
                    return Forbid("you're not authorized");
                }
                TaskReminder.IsReminderCompleted = true;
                TaskReminder.ReminderCompletedDate = DateTime.UtcNow;
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
                TaskReminderQuery = (IQueryable<TaskReminder>)await BussinesService.GetAllAsync(null!, ["TaskModel", "TaskModel.AssignedUsers", "TaskModel.AssignedUsers.User"]);
            else if (User.IsInRole(AppRoles.Admin))
                TaskReminderQuery = (IQueryable<TaskReminder>)await BussinesService
                    .GetAllAsync(w => w.TaskModel.AssignedUsers.Any(x => x.User.DepartmentId == user.DepartmentId)
                    , ["AssignedUsers", "Reminders", "AssignedUsers.User"]);
            else
                TaskReminderQuery = (IQueryable<TaskReminder>)await BussinesService
                                   .GetAllAsync(w => w.TaskModel.AssignedUsers.Any(x => x.UserId == user.Id)
                                   , ["AssignedUsers", "Reminders", "AssignedUsers.User"]);

            if (!string.IsNullOrEmpty(searchValue))
            {
                TaskReminderQuery = TaskReminderQuery.Where(b => b.TaskModel.AssignedUsers.Any(x => x.User.FullName.Contains(searchValue!))
                || (string.IsNullOrEmpty(b.Title) || b.Title.Contains(searchValue!))
                );
            }

            var TaskReminder = TaskReminderQuery.ToList();

            if (fromDate.HasValue)
                TaskReminder = TaskReminder.Where(x => x.DateCreated >= fromDate.Value).ToList();
            if (toDate.HasValue)
                TaskReminder = TaskReminder.Where(x => x.DateCreated <= toDate.Value).ToList();

            var recordsTotal = TaskReminder.Count;
            TaskReminder = TaskReminder.ToList();
            var data = TaskReminder.Skip(skip).Take(pageSize).ToList();

            var mappedData = _mapper.Map<IEnumerable<TaskReminderViewModel>>(TaskReminder);


            var jsonData = new { recordsFiltered = recordsTotal, recordsTotal, data = mappedData };

            return Ok(jsonData);
        }
    }

}
