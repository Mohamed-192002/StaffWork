namespace StaffWork.Core.Paramaters
{
    public class WorkDailyViewModel
    {
        public virtual int Id { get; set; }
        public virtual DateTime Date { get; set; }
        public string Note { get; set; }
        public virtual string WorkType { get; set; }
        public virtual string FullName { get; set; }
        public virtual string DeptName { get; set; }
        public virtual string Status { get; set; }
        public virtual DateTime DateCreated { get; set; }

        //  public virtual bool IsDisabled { get; set; }
        public virtual bool IsCompleted { get; set; }
        public virtual DateTime? CompletionDate { get; set; }
        public string? TimeDifferenceFormatted { get; set; }

    }
}
