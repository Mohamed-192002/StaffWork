using System;
using StaffWork.Core.Models.Base;

namespace StaffWork.Core.Models;

public class TaskModel : BaseEntity
{
    public string? Title { get; set; } = string.Empty;
    public string? Notes { get; set; } = string.Empty;

    public bool IsReceived { get; set; } = false; // حالة استلام عامة
    public DateTime? DateReceived { get; set; }
    public string? ReceivedByUserId { get; set; }
    public bool IsCompleted { get; set; } = false; // حالة إنجاز عامة
    public DateTime? DateCompleted { get; set; }

    public ICollection<TaskUser> AssignedUsers { get; set; } = [];
    public ICollection<TaskReminder> Reminders { get; set; } = [];
    public ICollection<TaskFile> TaskFiles { get; set; } = [];

}
public class TaskFile : BaseEntity
{
    public string FileUrl { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public int TaskModelId { get; set; }
    public TaskModel TaskModel { get; set; } = null!;
}

