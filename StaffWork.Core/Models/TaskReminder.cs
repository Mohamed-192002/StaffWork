using System.ComponentModel.DataAnnotations.Schema;
using StaffWork.Core.Models.Base;

namespace StaffWork.Core.Models;

public class TaskReminder : BaseEntity
{
    public string? Title { get; set; } = string.Empty;
    public int TaskModelId { get; set; }
    [ForeignKey(nameof(TaskModelId))]
    public TaskModel TaskModel { get; set; }

    public string CreatedByUserId { get; set; }
    [ForeignKey(nameof(CreatedByUserId))]
    public User CreatedByUser { get; set; }

    public DateTime ReminderDate { get; set; }

    public string? Notes { get; set; } = string.Empty; 

    public bool IsReminderCompleted { get; set; } // حالة التذكير
    public DateTime? ReminderCompletedDate { get; set; } // تاريخ إكمال التذكير

    public ICollection<Notification> Notifications { get; set; } = [];
    public ICollection<TaskReminderFile> TaskReminderFiles { get; set; } = [];

    public string? JobId { get; set; }
}
public class TaskReminderFile : BaseEntity
{
    public string FileUrl { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public int TaskReminderId { get; set; }
    public TaskReminder TaskReminder { get; set; } = null!;
}

