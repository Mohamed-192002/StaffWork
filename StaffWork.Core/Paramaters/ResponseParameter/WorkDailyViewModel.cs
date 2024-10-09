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
    }
}
