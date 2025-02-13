using StaffWork.Core.Models.Base;


namespace StaffWork.Core.Models
{
    public class Vacation : BaseEntity
    {
        public int EmployeeId { get; set; }
        public Employee Employee{ get; set; }
        public int VacationTypeId { get; set; }
        public VacationType VacationType{ get; set; }
        public int VacationDays { get; set; }
        public bool IsReturned { get; set; } = false;
        public virtual DateTime StartDate { get; set; }
        public virtual DateTime? ReturnedDate { get; set; }
        public virtual string? Description { get; set; }
    }
}
