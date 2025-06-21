using StaffWork.Core.Models.Base;
using System.ComponentModel.DataAnnotations.Schema;

namespace StaffWork.Core.Models;

public class PersonalReminder : BaseEntity
{
    public string? Title { get; set; } = string.Empty;
    public string CreatedByUserId { get; set; }
    [ForeignKey(nameof(CreatedByUserId))]
    public User CreatedByUser { get; set; }

    public DateTime ReminderDate { get; set; }

    public string? Notes { get; set; } = string.Empty;

    public bool IsReminderCompleted { get; set; } // حالة التذكير
    public DateTime? ReminderCompletedDate { get; set; } // تاريخ إكمال التذكير

    public ICollection<Notification> Notifications { get; set; } = [];
    public ICollection<PersonalReminderFile> PersonalReminderFiles { get; set; } = [];

    public string? JobId { get; set; }
}
public class PersonalReminderFile : BaseEntity
{
    public string FileUrl { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public int PersonalReminderId { get; set; }
    public PersonalReminder PersonalReminder { get; set; } = null!;
}