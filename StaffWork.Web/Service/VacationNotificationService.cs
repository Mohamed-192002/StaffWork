using Microsoft.AspNetCore.SignalR;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using StaffWork.Web.Service;

//public class VacationNotificationService
//{
//	private readonly IHubContext<NotificationHub> _hubContext;
//	private readonly IServicesBase<Notification> _NotificationService;
//	private readonly IServicesBase<Vacation> _VacationService;

//	public VacationNotificationService(IHubContext<NotificationHub> hubContext, IServicesBase<Notification> notificationService, IServicesBase<Vacation> vacationService)
//	{
//		_hubContext = hubContext;
//		_NotificationService = notificationService;
//		_VacationService = vacationService;
//	}

//	public async Task CheckVacationEndDates()
//	{
//		var vacationsEndingSoon = await _VacationService.GetAllAsync
//			(v => !v.IsReturned && v.StartDate.Value.AddDays(v.VacationDays) <= DateTime.Now.AddDays(1), ["Employee", "VacationType"]);


//		foreach (var vacation in vacationsEndingSoon)
//		{
//			string message = $"🔔 تنبيه: إجازة الموظف {vacation.Employee.FullName} ستنتهي غدًا!";
//			// Send notification logic
//			var notification = new Notification
//			{
//				Title = "Vacation Ending Soon",
//				Content = message,
//				DateCreated = DateTime.Now,
//				IsRead = false
//			};
//			await _NotificationService.InsertAsync(notification);
//		}
//		await _hubContext.Clients.All.SendAsync("ReceiveNotification", "تم إرسال تنبيهات الإجازات القادمة");
//	}


//}
