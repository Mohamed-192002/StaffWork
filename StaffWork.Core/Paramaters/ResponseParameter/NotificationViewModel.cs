namespace StaffWork.Core.Paramaters
{
    public class NotificationViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public bool IsRead { get; set; }
        public virtual DateTime DateCreated { get; set; }
        public VacationViewModel? Vacation { get; set; }
        public TaskReminderViewModel? TaskReminder { get; set; }

    }
}
