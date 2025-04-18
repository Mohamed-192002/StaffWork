namespace StaffWork.Core.Paramaters
{
    public class TaskReminderViewModel
    {
        public string? Title { get; set; } = string.Empty;
        public int TaskModelTitle { get; set; }
        public string CreatedByUserName { get; set; }
        public DateTime ReminderDate { get; set; }
        public bool IsReminderCompleted { get; set; }
        public DateTime? ReminderCompletedDate { get; set; }
    }
}
