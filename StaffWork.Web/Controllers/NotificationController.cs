using AutoMapper;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StaffWork.Api.Controllers;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using StaffWork.Core.Paramaters;

namespace StaffWork.Web.Controllers
{
    public class NotificationController : ApiBaseController<Notification>
    {
        public NotificationController(IServicesBase<Notification> servicesBase, IMapper mapper)
            : base(servicesBase, mapper)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadNotificationCount()
        {
            var unreadCount = await BussinesService.GetAllAsync(n => !n.IsRead);
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
            fromDate ??= DateTime.Today; // Default to today if null
            toDate ??= DateTime.Today.AddDays(1).AddSeconds(-1); // Include the full day

            //var model = await BussinesService.GetAllAsync(
            //    x => x.DateCreated >= fromDate.Value && x.DateCreated <= toDate.Value,
            //    ["Vacation", "Vacation.Employee", "Vacation.VacationType"]
            //);
            IQueryable<Notification> NotificationQuery;
            NotificationQuery = (IQueryable<Notification>)await BussinesService.GetAllAsync(null!, ["Vacation", "Vacation.Employee", "Vacation.VacationType"]
            , orderBy: x => x.DateCreated, orderByDirection: "DESC");

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
