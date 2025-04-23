using Hangfire;
using Microsoft.AspNetCore.SignalR;
using StaffWork.Core.Interfaces;
using StaffWork.Core.Models;
using StaffWork.Core.Paramaters;
using StaffWork.Web.Service;

public class NotifiJob
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IServicesBase<Notification> _NotificationService;
    private readonly IServicesBase<TaskModel> _TaskModelService;

    public NotifiJob(IHubContext<NotificationHub> hubContext, IServicesBase<Notification> notificationService, IServicesBase<TaskModel> taskModelService)
    {
        _hubContext = hubContext;
        _NotificationService = notificationService;
        _TaskModelService = taskModelService;
    }
    public void ScheduleNotifiJob(Vacation vacation)
    {
        BackgroundJob.Enqueue(() => CheckVacationEndDates(vacation));
    }
    public async Task CheckVacationEndDates(Vacation vacation)
    {
        string message = $"🔔 تنبيه: إجازة الموظف {vacation.Employee.FullName} ستنتهي فى {vacation.EndDate}!";
        // Send notification logic
        var notification = new Notification
        {
            Title = "اشعار انتهاء اجازه",
            Content = message,
            DateCreated = DateTime.Now,
            IsRead = false,
            VacationId = vacation.Id
        };
        await _NotificationService.InsertAsync(notification);

        //  await _hubContext.Clients.All.SendAsync("ReceiveNotification", "تم إرسال تنبيهات الإجازات القادمة");
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
    }
    public void ScheduleNotifiTaskReminderJob(TaskReminderViewModel taskReminder)
    {
        BackgroundJob.Enqueue(() => CheckTaskReminderDates(taskReminder));

    }
    public async Task CheckTaskReminderDates(TaskReminderViewModel taskReminder)
    {
        var taskModel = await _TaskModelService.GetAsync(x => x.Id == taskReminder.TaskModelId);

        string message = $"🔔 تنبيه: تذكير بشأن المهمه {taskModel.Title}";
        // Send notification logic
        var notification = new Notification
        {
            Title = "اشعار بشأن مهمه",
            Content = message,
            DateCreated = DateTime.Now,
            IsRead = false,
            TaskReminderId = taskReminder.Id
        };
        await _NotificationService.InsertAsync(notification);

        //  await _hubContext.Clients.All.SendAsync("ReceiveNotification", "تم إرسال تنبيهات الإجازات القادمة");
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", notification);
    }
}
