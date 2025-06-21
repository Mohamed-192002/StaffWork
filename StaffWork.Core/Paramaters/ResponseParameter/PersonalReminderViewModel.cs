namespace StaffWork.Core.Paramaters
{
    public class PersonalReminderViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string CreatedByUserName { get; set; }
        public DateTime ReminderDate { get; set; }
        public bool IsReminderCompleted { get; set; }
        public DateTime? ReminderCompletedDate { get; set; }
        public string? Notes { get; set; }

        public IList<TaskFileDisplay> ExistingFiles { get; set; } = [];
    }
}
