using System.Security.Claims;
using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StaffWork.Api.Controllers;
using StaffWork.Core.Consts;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using StaffWork.Core.Paramaters;
using StaffWork.Infrastructure.Implementations;

namespace StaffWork.Web.Controllers
{
    public class NotificationController : ApiBaseController<Notification>
    {
        public readonly IServicesBase<User> UserService;

        public NotificationController(IServicesBase<Notification> servicesBase, IMapper mapper, IServicesBase<User> userService)
            : base(servicesBase, mapper)
        {
            UserService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadNotificationCount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var unreadCount = await BussinesService
                .GetAllAsync(n =>
                !n.IsRead &&
                ((n.TaskReminderId != null && n.TaskReminder.TaskModel.AssignedUsers.Any(a => a.UserId == userId)) || (n.PersonalReminderId != null && n.PersonalReminder.CreatedByUserId == userId)),
                    ["PersonalReminder", "TaskReminder", "TaskReminder.TaskModel", "TaskReminder.TaskModel.AssignedUsers"]);
            return Json(new { count = unreadCount.Count() });
        }
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await BussinesService.GetAsync(x => x.Id == id);
            if (notification == null)
                return NotFound();

            notification.IsRead = true;
            await BussinesService.UpdateAsync(id, notification);

            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> Index(DateTime? fromDate, DateTime? toDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();
            var user = await UserService.GetAsync(x => x.Id == userId, ["Department"]);

            fromDate ??= DateTime.Today; // Default to today if null
            toDate ??= DateTime.Today.AddDays(1).AddSeconds(-1); // Include the full day


            IQueryable<Notification> NotificationQuery;
            NotificationQuery = (IQueryable<Notification>)await BussinesService.GetAllAsync(
                          x =>
                              (
                                   x.TaskReminderId != null &&
                                   x.TaskReminder != null &&
                                   x.TaskReminder.TaskModel != null &&
                                   x.TaskReminder.TaskModel.AssignedUsers.Any(a => a.UserId == userId)
                               )
                              ||
                              (
                                   x.PersonalReminderId != null &&
                                   x.PersonalReminder != null &&
                                   x.PersonalReminder.CreatedByUserId == userId
                               ),
                      includes: [

                        "PersonalReminder",
                        "TaskReminder",
                        "TaskReminder.TaskModel",
                        "TaskReminder.TaskModel.AssignedUsers",
                        "TaskReminder.TaskModel.AssignedUsers.User"
                      ],
                      orderBy: x => x.DateCreated,
                      orderByDirection: "DESC"
                  );

            var model = NotificationQuery.Take(5000).ToList();

            ViewBag.FromDate = fromDate.Value.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate.Value.ToString("yyyy-MM-dd");

            var viewModel = _mapper.Map<IEnumerable<NotificationViewModel>>(model);

            return View(viewModel);
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> EditAsync(NotificationViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var Notification = await BussinesService.GetAsync(d => d.Id == viewModel.Id);
            if (Notification == null)
                return NotFound();
            _mapper.Map(viewModel, Notification);

            await BussinesService.UpdateAsync(Notification.Id, Notification);
            return RedirectToAction("Index", _mapper.Map<NotificationViewModel>(Notification));
        }
    }

}
