namespace StaffWork.Core.Paramaters
{
    public class TaskReminderViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string TaskModelId { get; set; }
        public string TaskModelTitle { get; set; }
        public string CreatedByUserName { get; set; }
        public DateTime ReminderDate { get; set; }
        public bool IsReminderCompleted { get; set; }
        public DateTime? ReminderCompletedDate { get; set; }
        public string? Notes { get; set; }
    }
}
