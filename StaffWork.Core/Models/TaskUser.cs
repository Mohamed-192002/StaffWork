using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StaffWork.Core.Models.Base;

namespace StaffWork.Core.Models;

public class TaskUser : BaseEntity
{
    public int TaskModelId { get; set; }
    [ForeignKey(nameof(TaskModelId))]
    public TaskModel TaskModel { get; set; }

    public string UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
}

