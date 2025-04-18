namespace StaffWork.Core.Paramaters
{
    public class TaskReminderFormViewModel
    {
        public int Id { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTime ReminderDate { get; set; }
        public bool IsReminderCompleted { get; set; } // حالة التذكير
        public DateTime? ReminderCompletedDate { get; set; } // تاريخ إكمال التذكير
        public List<int> AssignedUsersIds { get; set; } = new();
        public List<int> TaskFilesIds { get; set; } = new();
    }
}
