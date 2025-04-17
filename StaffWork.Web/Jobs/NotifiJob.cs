using Hangfire;
using Microsoft.AspNetCore.SignalR;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using StaffWork.Web.Service;

public class NotifiJob
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IServicesBase<Notification> _NotificationService;
    public NotifiJob(IHubContext<NotificationHub> hubContext, IServicesBase<Notification> notificationService)
    {
        _hubContext = hubContext;
        _NotificationService = notificationService;
    }
    //public void ScheduleNotifiJob(Vacation vacation)
    //{
    //    BackgroundJob.Enqueue(() => CheckVacationEndDates(vacation));

    //}
    //public async Task CheckVacationEndDates(Vacation vacation)
    //{
    //    string message = $"🔔 تنبيه: إجازة الموظف {vacation.Employee.FullName} ستنتهي فى {vacation.EndDate}!";
    //    // Send notification logic
    //    var notification = new Notification
    //    {
    //        Title = "اشعار انتهاء اجازه",
    //        Content = message,
    //        DateCreated = DateTime.Now,
    //        IsRead = false,
    //        VacationId = vacation.Id
    //    };
    //    await _NotificationService.InsertAsync(notification);

    //  //  await _hubContext.Clients.All.SendAsync("ReceiveNotification", "تم إرسال تنبيهات الإجازات القادمة");
    //    await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
    //}
}
