using System.ComponentModel.DataAnnotations.Schema;
using StaffWork.Core.Models.Base;

namespace StaffWork.Core.Models;

public class TaskReminder : BaseEntity
{
    public int TaskModelId { get; set; }
    [ForeignKey(nameof(TaskModelId))]
    public TaskModel TaskModel { get; set; }

    public string CreatedByUserId { get; set; }
    [ForeignKey(nameof(CreatedByUserId))]
    public User CreatedByUser { get; set; }

    public DateTime ReminderDate { get; set; }

    public bool IsReminderCompleted { get; set; } // حالة التذكير

    public ICollection<Notification> Notifications { get; set; } = [];
}

