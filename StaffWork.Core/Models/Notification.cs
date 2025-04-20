using System.ComponentModel.DataAnnotations.Schema;
using StaffWork.Core.Models.Base;

namespace StaffWork.Core.Models
{
    public class Notification : BaseEntity
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public bool IsRead { get; set; }
        public int? TaskReminderId { get; set; }
        [ForeignKey(nameof(TaskReminderId))]
        public TaskReminder? TaskReminder { get; set; }
    }
}
